using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TMKMiniApp.Models.JsonModels;

namespace TMKMiniApp.Services
{
    /// <summary>
    /// Автоматизированный сервис синхронизации данных каталога продукции
    /// Обрабатывает дельтовые обновления и выполняет ночную синхронизацию
    /// </summary>
    public class AutomatedDataSyncService : IAutomatedDataSyncService, IHostedService, IDisposable
    {
        private readonly ILogger<AutomatedDataSyncService> _logger;
        private readonly IProductService _productService;
        private readonly IDataSyncService _dataSyncService;
        private readonly IWebHostEnvironment _environment;
        private readonly JsonDataService _jsonDataService;
        
        private readonly string _updatesPath;
        private readonly string _archivePath;
        private readonly string _dataPath;
        private readonly SemaphoreSlim _processingSemaphore = new(1, 1);
        
        private Timer? _nightlySyncTimer;
        private FileSystemWatcher? _fileWatcher;
        private bool _disposed = false;

        public AutomatedDataSyncService(
            ILogger<AutomatedDataSyncService> logger,
            IProductService productService,
            IDataSyncService dataSyncService,
            IWebHostEnvironment environment,
            JsonDataService jsonDataService)
        {
            _logger = logger;
            _productService = productService;
            _dataSyncService = dataSyncService;
            _environment = environment;
            _jsonDataService = jsonDataService;
            
            // Определяем пути к папкам
            _updatesPath = Path.Combine(_environment.ContentRootPath, "..", "updates");
            _archivePath = Path.Combine(_updatesPath, "processed");
            _dataPath = Path.Combine(_environment.ContentRootPath, "Data");
            
            // Создаем папки если их нет
            Directory.CreateDirectory(_updatesPath);
            Directory.CreateDirectory(_archivePath);
        }

        /// <summary>
        /// Запускает сервис автоматизированной синхронизации
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Запуск автоматизированного сервиса синхронизации данных");
                
                // Запускаем мониторинг дельтовых файлов
                await StartDeltaMonitoringAsync();
                
                // Запускаем ночную синхронизацию
                StartNightlySync();
                
                _logger.LogInformation("Автоматизированный сервис синхронизации запущен успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запуске автоматизированного сервиса синхронизации");
                throw;
            }
        }

        /// <summary>
        /// Останавливает сервис автоматизированной синхронизации
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Остановка автоматизированного сервиса синхронизации");
                
                _fileWatcher?.Dispose();
                _nightlySyncTimer?.Dispose();
                
                _logger.LogInformation("Автоматизированный сервис синхронизации остановлен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при остановке автоматизированного сервиса синхронизации");
            }
        }

        /// <summary>
        /// Запускает мониторинг папки updates для обработки дельтовых файлов
        /// </summary>
        private async Task StartDeltaMonitoringAsync()
        {
            try
            {
                _logger.LogInformation("Запуск мониторинга дельтовых файлов: {UpdatesPath}", _updatesPath);
                
                _fileWatcher = new FileSystemWatcher(_updatesPath)
                {
                    Filter = "*.json",
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = false
                };
                
                _fileWatcher.Created += OnFileCreated;
                _fileWatcher.Changed += OnFileChanged;
                _fileWatcher.Error += OnFileWatcherError;
                
                // Обрабатываем существующие файлы
                await ProcessExistingDeltaFilesAsync();
                
                _logger.LogInformation("Мониторинг дельтовых файлов запущен успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запуске мониторинга дельтовых файлов");
                throw;
            }
        }

        /// <summary>
        /// Запускает ночную синхронизацию полного каталога
        /// </summary>
        private void StartNightlySync()
        {
            try
            {
                _logger.LogInformation("Запуск ночной синхронизации полного каталога");
                
                // Вычисляем время до следующей полуночи
                var now = DateTime.Now;
                var nextMidnight = now.Date.AddDays(1);
                var timeUntilMidnight = nextMidnight - now;
                
                _nightlySyncTimer = new Timer(
                    async _ => await DailyFullCatalogSyncAsync(),
                    null,
                    timeUntilMidnight,
                    TimeSpan.FromDays(1) // Повторяем каждые 24 часа
                );
                
                _logger.LogInformation("Ночная синхронизация запланирована на {NextSync}", nextMidnight);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запуске ночной синхронизации");
                throw;
            }
        }

        /// <summary>
        /// Обрабатывает существующие дельтовые файлы при запуске
        /// </summary>
        private async Task ProcessExistingDeltaFilesAsync()
        {
            try
            {
                var existingFiles = Directory.GetFiles(_updatesPath, "*.json");
                _logger.LogInformation("Найдено {Count} существующих дельтовых файлов для обработки", existingFiles.Length);
                
                foreach (var filePath in existingFiles)
                {
                    await ProcessDeltaFileAsync(filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке существующих дельтовых файлов");
            }
        }

        /// <summary>
        /// Обработчик создания нового файла
        /// </summary>
        private async void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            await ProcessDeltaFileAsync(e.FullPath);
        }

        /// <summary>
        /// Обработчик изменения файла
        /// </summary>
        private async void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            await ProcessDeltaFileAsync(e.FullPath);
        }

        /// <summary>
        /// Обработчик ошибок FileSystemWatcher
        /// </summary>
        private void OnFileWatcherError(object sender, ErrorEventArgs e)
        {
            _logger.LogError(e.GetException(), "Ошибка FileSystemWatcher");
        }

        /// <summary>
        /// Обрабатывает дельтовый файл
        /// </summary>
        private async Task ProcessDeltaFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            await _processingSemaphore.WaitAsync();
            try
            {
                var fileName = Path.GetFileName(filePath);
                _logger.LogInformation("Начало обработки дельтового файла: {FileName}", fileName);

                // Определяем тип файла по имени
                if (fileName.Contains("prices"))
                {
                    await ProcessPriceDeltaFileAsync(filePath);
                }
                else if (fileName.Contains("remnants"))
                {
                    await ProcessStockDeltaFileAsync(filePath);
                }
                else
                {
                    _logger.LogWarning("Неизвестный тип дельтового файла: {FileName}", fileName);
                    return;
                }

                // Архивируем обработанный файл
                await ArchiveProcessedFileAsync(filePath);
                
                _logger.LogInformation("Дельтовый файл {FileName} успешно обработан и заархивирован", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке дельтового файла {FilePath}", filePath);
            }
            finally
            {
                _processingSemaphore.Release();
            }
        }

        /// <summary>
        /// Обрабатывает дельтовый файл цен
        /// </summary>
        public async Task ProcessPriceDeltaFileAsync(string filePath)
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var pricesRoot = JsonSerializer.Deserialize<PricesRoot>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (pricesRoot?.ArrayOfPricesEl == null)
                {
                    _logger.LogWarning("Неверный формат файла цен: {FilePath}", filePath);
                    return;
                }

                var processedCount = 0;
                foreach (var priceDelta in pricesRoot.ArrayOfPricesEl)
                {
                    try
                    {
                        await ApplyDeltaToPriceAsync(priceDelta);
                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при применении дельты цены для товара {ProductId}", priceDelta.ID);
                    }
                }

                _logger.LogInformation("Обработано {Count} записей цен из файла {FilePath}", processedCount, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке дельтового файла цен {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Обрабатывает дельтовый файл остатков
        /// </summary>
        public async Task ProcessStockDeltaFileAsync(string filePath)
        {
            try
            {
                var json = await File.ReadAllTextAsync(filePath);
                var remnantsRoot = JsonSerializer.Deserialize<RemnantsRoot>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (remnantsRoot?.ArrayOfRemnantsEl == null)
                {
                    _logger.LogWarning("Неверный формат файла остатков: {FilePath}", filePath);
                    return;
                }

                var processedCount = 0;
                foreach (var remnantDelta in remnantsRoot.ArrayOfRemnantsEl)
                {
                    try
                    {
                        await ApplyDeltaToStockAsync(remnantDelta);
                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при применении дельты остатка для товара {ProductId}", remnantDelta.ID);
                    }
                }

                _logger.LogInformation("Обработано {Count} записей остатков из файла {FilePath}", processedCount, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке дельтового файла остатков {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Применяет дельту к цене товара
        /// </summary>
        public async Task ApplyDeltaToPriceAsync(PricesEl priceDelta)
        {
            try
            {
                // Получаем текущие данные о ценах
                var currentPriceData = await _productService.GetPriceDataAsync(priceDelta.ID);
                if (currentPriceData == null)
                {
                    _logger.LogWarning("Товар с ID {ProductId} не найден для применения дельты цены", priceDelta.ID);
                    return;
                }

                // Применяем дельты к ценам (новое_значение = текущее_значение + дельта)
                var newPriceT = Math.Max(0, currentPriceData.PriceT + priceDelta.PriceT);
                var newPriceM = Math.Max(0, currentPriceData.PriceM + priceDelta.PriceM);
                var newPriceLimitT1 = Math.Max(0, currentPriceData.PriceLimitT1 + priceDelta.PriceLimitT1);
                var newPriceT1 = Math.Max(0, currentPriceData.PriceT1 + priceDelta.PriceT1);
                var newPriceLimitT2 = Math.Max(0, currentPriceData.PriceLimitT2 + priceDelta.PriceLimitT2);
                var newPriceT2 = Math.Max(0, currentPriceData.PriceT2 + priceDelta.PriceT2);
                var newPriceLimitM1 = Math.Max(0, currentPriceData.PriceLimitM1 + priceDelta.PriceLimitM1);
                var newPriceM1 = Math.Max(0, currentPriceData.PriceM1 + priceDelta.PriceM1);
                var newPriceLimitM2 = Math.Max(0, currentPriceData.PriceLimitM2 + priceDelta.PriceLimitM2);
                var newPriceM2 = Math.Max(0, currentPriceData.PriceM2 + priceDelta.PriceM2);

                // Обновляем цены в базе данных
                await _productService.UpdateProductPriceAsync(
                    priceDelta.ID, 
                    priceDelta.IDStock, 
                    (decimal)newPriceT, 
                    (decimal)newPriceM
                );

                _logger.LogDebug("Применена дельта цены для товара {ProductId}: T={NewPriceT}, M={NewPriceM}", 
                    priceDelta.ID, newPriceT, newPriceM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при применении дельты цены для товара {ProductId}", priceDelta.ID);
                throw;
            }
        }

        /// <summary>
        /// Применяет дельту к остатку товара
        /// </summary>
        public async Task ApplyDeltaToStockAsync(RemnantsEl remnantDelta)
        {
            try
            {
                // Получаем текущие данные об остатках
                var currentRemnantData = await _productService.GetRemnantDataAsync(remnantDelta.ID);
                if (currentRemnantData == null)
                {
                    _logger.LogWarning("Товар с ID {ProductId} не найден для применения дельты остатка", remnantDelta.ID);
                    return;
                }

                // Применяем дельты к остаткам (новое_значение = текущее_значение + дельта)
                var newStockT = Math.Max(0, currentRemnantData.InStockT + remnantDelta.InStockT);
                var newStockM = Math.Max(0, currentRemnantData.InStockM + remnantDelta.InStockM);
                var newSoonArriveT = Math.Max(0, currentRemnantData.SoonArriveT + remnantDelta.SoonArriveT);
                var newSoonArriveM = Math.Max(0, currentRemnantData.SoonArriveM + remnantDelta.SoonArriveM);

                // Обновляем остатки в базе данных
                await _productService.UpdateProductStockAsync(
                    remnantDelta.ID, 
                    remnantDelta.IDStock, 
                    (decimal)newStockT, 
                    (decimal)newStockM
                );

                _logger.LogDebug("Применена дельта остатка для товара {ProductId}: T={NewStockT}, M={NewStockM}", 
                    remnantDelta.ID, newStockT, newStockM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при применении дельты остатка для товара {ProductId}", remnantDelta.ID);
                throw;
            }
        }

        /// <summary>
        /// Архивирует обработанный файл
        /// </summary>
        private async Task ArchiveProcessedFileAsync(string filePath)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);
                var archivePath = Path.Combine(_archivePath, $"{DateTime.Now:yyyyMMdd_HHmmss}_{fileName}");
                
                await Task.Run(() => File.Move(filePath, archivePath));
                _logger.LogDebug("Файл {FileName} заархивирован в {ArchivePath}", fileName, archivePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при архивировании файла {FilePath}", filePath);
                // Если не удалось заархивировать, удаляем файл
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception deleteEx)
                {
                    _logger.LogError(deleteEx, "Ошибка при удалении файла {FilePath}", filePath);
                }
            }
        }

        /// <summary>
        /// Выполняет ночную синхронизацию полного каталога
        /// </summary>
        public async Task DailyFullCatalogSyncAsync()
        {
            await _processingSemaphore.WaitAsync();
            try
            {
                _logger.LogInformation("Начало ночной синхронизации полного каталога");
                
                // Синхронизируем номенклатуру
                await SyncFullNomenclatureAsync();
                
                // Синхронизируем цены
                await SyncFullPricesAsync();
                
                // Синхронизируем остатки
                await SyncFullRemnantsAsync();
                
                // Синхронизируем склады
                await SyncFullStocksAsync();
                
                // Синхронизируем типы
                await SyncFullTypesAsync();
                
                _logger.LogInformation("Ночная синхронизация полного каталога завершена успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при ночной синхронизации полного каталога");
            }
            finally
            {
                _processingSemaphore.Release();
            }
        }

        /// <summary>
        /// Синхронизирует полную номенклатуру
        /// </summary>
        private async Task SyncFullNomenclatureAsync()
        {
            try
            {
                var nomenclature = await _jsonDataService.LoadNomenclatureAsync();
                _logger.LogInformation("Синхронизация номенклатуры: {Count} записей", nomenclature.ArrayOfNomenclatureEl?.Count ?? 0);
                
                // Здесь можно добавить логику полной синхронизации номенклатуры
                // в зависимости от требований к базе данных
                
                _logger.LogInformation("Синхронизация номенклатуры завершена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при синхронизации номенклатуры");
                throw;
            }
        }

        /// <summary>
        /// Синхронизирует полные цены
        /// </summary>
        private async Task SyncFullPricesAsync()
        {
            try
            {
                var prices = await _jsonDataService.LoadPricesAsync();
                _logger.LogInformation("Синхронизация цен: {Count} записей", prices.ArrayOfPricesEl?.Count ?? 0);
                
                // Здесь можно добавить логику полной синхронизации цен
                // в зависимости от требований к базе данных
                
                _logger.LogInformation("Синхронизация цен завершена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при синхронизации цен");
                throw;
            }
        }

        /// <summary>
        /// Синхронизирует полные остатки
        /// </summary>
        private async Task SyncFullRemnantsAsync()
        {
            try
            {
                var remnants = await _jsonDataService.LoadRemnantsAsync();
                _logger.LogInformation("Синхронизация остатков: {Count} записей", remnants.ArrayOfRemnantsEl?.Count ?? 0);
                
                // Здесь можно добавить логику полной синхронизации остатков
                // в зависимости от требований к базе данных
                
                _logger.LogInformation("Синхронизация остатков завершена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при синхронизации остатков");
                throw;
            }
        }

        /// <summary>
        /// Синхронизирует полные склады
        /// </summary>
        private async Task SyncFullStocksAsync()
        {
            try
            {
                var stocks = await _jsonDataService.LoadStocksAsync();
                _logger.LogInformation("Синхронизация складов: {Count} записей", stocks.ArrayOfStockEl?.Count ?? 0);
                
                // Здесь можно добавить логику полной синхронизации складов
                // в зависимости от требований к базе данных
                
                _logger.LogInformation("Синхронизация складов завершена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при синхронизации складов");
                throw;
            }
        }

        /// <summary>
        /// Синхронизирует полные типы
        /// </summary>
        private async Task SyncFullTypesAsync()
        {
            try
            {
                var types = await _jsonDataService.LoadTypesAsync();
                _logger.LogInformation("Синхронизация типов: {Count} записей", types.ArrayOfTypeEl?.Count ?? 0);
                
                // Здесь можно добавить логику полной синхронизации типов
                // в зависимости от требований к базе данных
                
                _logger.LogInformation("Синхронизация типов завершена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при синхронизации типов");
                throw;
            }
        }

        /// <summary>
        /// Получает информацию о последних дельтах
        /// </summary>
        public async Task<DeltaSyncInfo> GetLastDeltaSyncInfoAsync()
        {
            try
            {
                var archiveFiles = Directory.GetFiles(_archivePath, "*.json")
                    .OrderByDescending(f => File.GetCreationTime(f))
                    .Take(10)
                    .ToArray();

                return new DeltaSyncInfo
                {
                    LastSyncTime = archiveFiles.Any() ? File.GetCreationTime(archiveFiles.First()) : DateTime.MinValue,
                    ProcessedFilesCount = archiveFiles.Length,
                    RecentFiles = archiveFiles.Select(f => Path.GetFileName(f)).ToArray()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении информации о последних дельтах");
                return new DeltaSyncInfo();
            }
        }

        /// <summary>
        /// Проверяет состояние синхронизации
        /// </summary>
        public async Task<SyncStatus> GetSyncStatusAsync()
        {
            try
            {
                var deltaInfo = await GetLastDeltaSyncInfoAsync();
                var isProcessing = _processingSemaphore.CurrentCount == 0;
                
                return new SyncStatus
                {
                    IsRunning = true,
                    IsProcessing = isProcessing,
                    LastDeltaSync = deltaInfo.LastSyncTime,
                    ProcessedFilesCount = deltaInfo.ProcessedFilesCount,
                    NextNightlySync = GetNextNightlySyncTime()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статуса синхронизации");
                return new SyncStatus { IsRunning = false };
            }
        }

        /// <summary>
        /// Получает время следующей ночной синхронизации
        /// </summary>
        private DateTime GetNextNightlySyncTime()
        {
            var now = DateTime.Now;
            var nextMidnight = now.Date.AddDays(1);
            return nextMidnight;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _fileWatcher?.Dispose();
                _nightlySyncTimer?.Dispose();
                _processingSemaphore?.Dispose();
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Информация о дельтовой синхронизации
    /// </summary>
    public class DeltaSyncInfo
    {
        public DateTime LastSyncTime { get; set; }
        public int ProcessedFilesCount { get; set; }
        public string[] RecentFiles { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// Статус синхронизации
    /// </summary>
    public class SyncStatus
    {
        public bool IsRunning { get; set; }
        public bool IsProcessing { get; set; }
        public DateTime LastDeltaSync { get; set; }
        public int ProcessedFilesCount { get; set; }
        public DateTime NextNightlySync { get; set; }
    }
}
