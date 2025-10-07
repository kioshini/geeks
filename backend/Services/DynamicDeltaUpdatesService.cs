using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TMKMiniApp.Models.JsonModels;

namespace TMKMiniApp.Services
{
    /// <summary>
    /// Сервис для динамического обновления цен и остатков товаров
    /// с учетом дельт из JSON-файлов в папке updates
    /// </summary>
    public class DynamicDeltaUpdatesService : IDynamicDeltaUpdatesService, IHostedService
    {
        private readonly ILogger<DynamicDeltaUpdatesService> _logger;
        private readonly IProductService _productService;
        private readonly IDataSyncService _dataSyncService;
        private readonly IWebHostEnvironment _environment;
        private FileSystemWatcher? _fileWatcher;
        private readonly string _updatesPath;
        private readonly string _archivePath;
        private readonly SemaphoreSlim _processingSemaphore = new(1, 1);

        public DynamicDeltaUpdatesService(
            ILogger<DynamicDeltaUpdatesService> logger,
            IProductService productService,
            IDataSyncService dataSyncService,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _productService = productService;
            _dataSyncService = dataSyncService;
            _environment = environment;
            
            // Определяем пути к папкам
            _updatesPath = Path.Combine(_environment.ContentRootPath, "..", "updates");
            _archivePath = Path.Combine(_updatesPath, "processed");
            
            // Создаем папки если их нет
            Directory.CreateDirectory(_updatesPath);
            Directory.CreateDirectory(_archivePath);
        }

        /// <summary>
        /// Запускает мониторинг папки updates для обработки новых файлов
        /// </summary>
        public async Task StartMonitoringAsync()
        {
            try
            {
                _logger.LogInformation("Запуск мониторинга папки updates: {UpdatesPath}", _updatesPath);

                _fileWatcher = new FileSystemWatcher(_updatesPath)
                {
                    Filter = "*.json",
                    EnableRaisingEvents = true,
                    NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite
                };

                _fileWatcher.Created += OnFileCreated;
                _fileWatcher.Changed += OnFileChanged;

                _logger.LogInformation("Мониторинг папки updates запущен успешно");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запуске мониторинга папки updates");
                throw;
            }
        }

        /// <summary>
        /// Останавливает мониторинг папки updates
        /// </summary>
        public async Task StopMonitoringAsync()
        {
            try
            {
                _logger.LogInformation("Остановка мониторинга папки updates");

                if (_fileWatcher != null)
                {
                    _fileWatcher.EnableRaisingEvents = false;
                    _fileWatcher.Created -= OnFileCreated;
                    _fileWatcher.Changed -= OnFileChanged;
                    _fileWatcher.Dispose();
                    _fileWatcher = null;
                }

                _logger.LogInformation("Мониторинг папки updates остановлен");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при остановке мониторинга папки updates");
            }
        }

        /// <summary>
        /// Обрабатывает файл с дельтами цен
        /// </summary>
        public async Task ProcessPricesDeltaFileAsync(string filePath)
        {
            await _processingSemaphore.WaitAsync();
            try
            {
                _logger.LogInformation("Начало обработки файла цен: {FilePath}", filePath);

                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("Файл не найден: {FilePath}", filePath);
                    return;
                }

                var jsonContent = await File.ReadAllTextAsync(filePath);
                var pricesRoot = JsonSerializer.Deserialize<PricesRoot>(jsonContent);

                if (pricesRoot?.ArrayOfPricesEl == null)
                {
                    _logger.LogWarning("Не удалось десериализовать файл цен: {FilePath}", filePath);
                    return;
                }

                var processedCount = 0;
                var errorCount = 0;

                foreach (var priceDelta in pricesRoot.ArrayOfPricesEl)
                {
                    try
                    {
                        await ProcessPriceDeltaAsync(priceDelta);
                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при обработке дельты цены для ID: {ProductId}, IDStock: {StockId}", 
                            priceDelta.ID, priceDelta.IDStock);
                        errorCount++;
                    }
                }

                _logger.LogInformation("Обработка файла цен завершена. Обработано: {ProcessedCount}, Ошибок: {ErrorCount}", 
                    processedCount, errorCount);

                // Перемещаем файл в архив
                await ArchiveFileAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критическая ошибка при обработке файла цен: {FilePath}", filePath);
            }
            finally
            {
                _processingSemaphore.Release();
            }
        }

        /// <summary>
        /// Обрабатывает файл с дельтами остатков
        /// </summary>
        public async Task ProcessRemnantsDeltaFileAsync(string filePath)
        {
            await _processingSemaphore.WaitAsync();
            try
            {
                _logger.LogInformation("Начало обработки файла остатков: {FilePath}", filePath);

                if (!File.Exists(filePath))
                {
                    _logger.LogWarning("Файл не найден: {FilePath}", filePath);
                    return;
                }

                var jsonContent = await File.ReadAllTextAsync(filePath);
                var remnantsRoot = JsonSerializer.Deserialize<RemnantsRoot>(jsonContent);

                if (remnantsRoot?.ArrayOfRemnantsEl == null)
                {
                    _logger.LogWarning("Не удалось десериализовать файл остатков: {FilePath}", filePath);
                    return;
                }

                var processedCount = 0;
                var errorCount = 0;

                foreach (var remnantDelta in remnantsRoot.ArrayOfRemnantsEl)
                {
                    try
                    {
                        await ProcessRemnantDeltaAsync(remnantDelta);
                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при обработке дельты остатка для ID: {ProductId}, IDStock: {StockId}", 
                            remnantDelta.ID, remnantDelta.IDStock);
                        errorCount++;
                    }
                }

                _logger.LogInformation("Обработка файла остатков завершена. Обработано: {ProcessedCount}, Ошибок: {ErrorCount}", 
                    processedCount, errorCount);

                // Перемещаем файл в архив
                await ArchiveFileAsync(filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критическая ошибка при обработке файла остатков: {FilePath}", filePath);
            }
            finally
            {
                _processingSemaphore.Release();
            }
        }

        /// <summary>
        /// Применяет дельту к цене товара
        /// </summary>
        public decimal ApplyPriceDelta(decimal currentPrice, decimal delta)
        {
            var newPrice = currentPrice + delta;
            return Math.Max(0, newPrice); // Минимум 0
        }

        /// <summary>
        /// Применяет дельту к остатку товара
        /// </summary>
        public decimal ApplyStockDelta(decimal currentStock, decimal delta)
        {
            var newStock = currentStock + delta;
            return Math.Max(0, newStock); // Минимум 0
        }

        #region Private Methods

        /// <summary>
        /// Обработчик события создания файла
        /// </summary>
        private async void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            await ProcessFileAsync(e.FullPath);
        }

        /// <summary>
        /// Обработчик события изменения файла
        /// </summary>
        private async void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // Небольшая задержка, чтобы файл полностью записался
            await Task.Delay(100);
            await ProcessFileAsync(e.FullPath);
        }

        /// <summary>
        /// Определяет тип файла и запускает соответствующую обработку
        /// </summary>
        private async Task ProcessFileAsync(string filePath)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);
                _logger.LogInformation("Обнаружен новый файл: {FileName}", fileName);

                if (fileName.StartsWith("prices_"))
                {
                    await ProcessPricesDeltaFileAsync(filePath);
                }
                else if (fileName.StartsWith("remnants_"))
                {
                    await ProcessRemnantsDeltaFileAsync(filePath);
                }
                else
                {
                    _logger.LogWarning("Неизвестный тип файла: {FileName}", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке файла: {FilePath}", filePath);
            }
        }

        /// <summary>
        /// Обрабатывает дельту цены для конкретного товара
        /// </summary>
        private async Task ProcessPriceDeltaAsync(PricesEl priceDelta)
        {
            _logger.LogDebug("Обработка дельты цены для товара ID: {ProductId}, IDStock: {StockId}", 
                priceDelta.ID, priceDelta.IDStock);

            try
            {
                var success = await _productService.UpdatePriceDeltaAsync(priceDelta.ID, priceDelta.IDStock, priceDelta);
                
                if (success)
                {
                    _logger.LogInformation("Успешно обновлена цена товара {ProductId}: PriceT={PriceT}, PriceM={PriceM}", 
                        priceDelta.ID, priceDelta.PriceT, priceDelta.PriceM);
                }
                else
                {
                    _logger.LogWarning("Не удалось обновить цену товара {ProductId}", priceDelta.ID);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке дельты цены для товара {ProductId}", priceDelta.ID);
            }
        }

        /// <summary>
        /// Обрабатывает дельту остатка для конкретного товара
        /// </summary>
        private async Task ProcessRemnantDeltaAsync(RemnantsEl remnantDelta)
        {
            _logger.LogDebug("Обработка дельты остатка для товара ID: {ProductId}, IDStock: {StockId}", 
                remnantDelta.ID, remnantDelta.IDStock);

            try
            {
                var success = await _productService.UpdateStockDeltaAsync(remnantDelta.ID, remnantDelta.IDStock, remnantDelta);
                
                if (success)
                {
                    _logger.LogInformation("Успешно обновлен остаток товара {ProductId}: InStockT={InStockT}, InStockM={InStockM}", 
                        remnantDelta.ID, remnantDelta.InStockT, remnantDelta.InStockM);
                }
                else
                {
                    _logger.LogWarning("Не удалось обновить остаток товара {ProductId}", remnantDelta.ID);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке дельты остатка для товара {ProductId}", remnantDelta.ID);
            }
        }

        /// <summary>
        /// Перемещает обработанный файл в архив
        /// </summary>
        private async Task ArchiveFileAsync(string filePath)
        {
            try
            {
                var fileName = Path.GetFileName(filePath);
                var archiveFilePath = Path.Combine(_archivePath, $"{DateTime.Now:yyyyMMdd_HHmmss}_{fileName}");
                
                File.Move(filePath, archiveFilePath);
                _logger.LogInformation("Файл перемещен в архив: {ArchivePath}", archiveFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при архивировании файла: {FilePath}", filePath);
            }
        }

        #endregion

        #region IHostedService Implementation

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await StartMonitoringAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopMonitoringAsync();
        }

        #endregion
    }
}
