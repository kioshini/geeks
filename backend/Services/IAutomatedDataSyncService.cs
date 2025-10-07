using TMKMiniApp.Models.JsonModels;

namespace TMKMiniApp.Services
{
    /// <summary>
    /// Интерфейс для автоматизированного сервиса синхронизации данных каталога
    /// </summary>
    public interface IAutomatedDataSyncService
    {
        /// <summary>
        /// Выполняет ночную синхронизацию полного каталога
        /// </summary>
        Task DailyFullCatalogSyncAsync();

        /// <summary>
        /// Обрабатывает дельтовый файл цен
        /// </summary>
        /// <param name="filePath">Путь к файлу с дельтами цен</param>
        Task ProcessPriceDeltaFileAsync(string filePath);

        /// <summary>
        /// Обрабатывает дельтовый файл остатков
        /// </summary>
        /// <param name="filePath">Путь к файлу с дельтами остатков</param>
        Task ProcessStockDeltaFileAsync(string filePath);

        /// <summary>
        /// Применяет дельту к цене товара
        /// </summary>
        /// <param name="priceDelta">Дельтовые данные цены</param>
        Task ApplyDeltaToPriceAsync(PricesEl priceDelta);

        /// <summary>
        /// Применяет дельту к остатку товара
        /// </summary>
        /// <param name="remnantDelta">Дельтовые данные остатка</param>
        Task ApplyDeltaToStockAsync(RemnantsEl remnantDelta);

        /// <summary>
        /// Получает информацию о последних дельтах
        /// </summary>
        Task<DeltaSyncInfo> GetLastDeltaSyncInfoAsync();

        /// <summary>
        /// Проверяет состояние синхронизации
        /// </summary>
        Task<SyncStatus> GetSyncStatusAsync();
    }
}
