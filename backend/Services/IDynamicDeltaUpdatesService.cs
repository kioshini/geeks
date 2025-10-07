using TMKMiniApp.Models.JsonModels;

namespace TMKMiniApp.Services
{
    /// <summary>
    /// Интерфейс для сервиса динамического обновления цен и остатков товаров
    /// с учетом дельт из JSON-файлов
    /// </summary>
    public interface IDynamicDeltaUpdatesService
    {
        /// <summary>
        /// Запускает мониторинг папки updates для обработки новых файлов
        /// </summary>
        Task StartMonitoringAsync();

        /// <summary>
        /// Останавливает мониторинг папки updates
        /// </summary>
        Task StopMonitoringAsync();

        /// <summary>
        /// Обрабатывает файл с дельтами цен
        /// </summary>
        /// <param name="filePath">Путь к файлу prices_*.json</param>
        Task ProcessPricesDeltaFileAsync(string filePath);

        /// <summary>
        /// Обрабатывает файл с дельтами остатков
        /// </summary>
        /// <param name="filePath">Путь к файлу remnants_*.json</param>
        Task ProcessRemnantsDeltaFileAsync(string filePath);

        /// <summary>
        /// Применяет дельту к цене товара
        /// </summary>
        /// <param name="currentPrice">Текущая цена</param>
        /// <param name="delta">Дельта (может быть отрицательной)</param>
        /// <returns>Новая цена (минимум 0)</returns>
        decimal ApplyPriceDelta(decimal currentPrice, decimal delta);

        /// <summary>
        /// Применяет дельту к остатку товара
        /// </summary>
        /// <param name="currentStock">Текущий остаток</param>
        /// <param name="delta">Дельта (может быть отрицательной)</param>
        /// <returns>Новый остаток (минимум 0)</returns>
        decimal ApplyStockDelta(decimal currentStock, decimal delta);
    }
}
