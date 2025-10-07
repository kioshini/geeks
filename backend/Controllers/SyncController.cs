using Microsoft.AspNetCore.Mvc;
using TMKMiniApp.Services;

namespace TMKMiniApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly IAutomatedDataSyncService _syncService;
        private readonly ILogger<SyncController> _logger;

        public SyncController(IAutomatedDataSyncService syncService, ILogger<SyncController> logger)
        {
            _syncService = syncService;
            _logger = logger;
        }

        /// <summary>
        /// Получить статус синхронизации
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult<SyncStatus>> GetSyncStatus()
        {
            try
            {
                var status = await _syncService.GetSyncStatusAsync();
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении статуса синхронизации");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получить информацию о последних дельтах
        /// </summary>
        [HttpGet("delta-info")]
        public async Task<ActionResult<DeltaSyncInfo>> GetDeltaInfo()
        {
            try
            {
                var deltaInfo = await _syncService.GetLastDeltaSyncInfoAsync();
                return Ok(deltaInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении информации о дельтах");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Запустить ночную синхронизацию вручную
        /// </summary>
        [HttpPost("trigger-nightly-sync")]
        public async Task<ActionResult> TriggerNightlySync()
        {
            try
            {
                _logger.LogInformation("Запуск ночной синхронизации по запросу пользователя");
                await _syncService.DailyFullCatalogSyncAsync();
                return Ok(new { message = "Ночная синхронизация запущена успешно" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запуске ночной синхронизации");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Обработать дельтовый файл цен
        /// </summary>
        [HttpPost("process-price-delta")]
        public async Task<ActionResult> ProcessPriceDelta([FromBody] ProcessDeltaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FilePath) || !System.IO.File.Exists(request.FilePath))
                {
                    return BadRequest("Файл не найден или путь не указан");
                }

                _logger.LogInformation("Обработка дельтового файла цен: {FilePath}", request.FilePath);
                await _syncService.ProcessPriceDeltaFileAsync(request.FilePath);
                
                return Ok(new { message = "Дельтовый файл цен обработан успешно" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке дельтового файла цен");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Обработать дельтовый файл остатков
        /// </summary>
        [HttpPost("process-stock-delta")]
        public async Task<ActionResult> ProcessStockDelta([FromBody] ProcessDeltaRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FilePath) || !System.IO.File.Exists(request.FilePath))
                {
                    return BadRequest("Файл не найден или путь не указан");
                }

                _logger.LogInformation("Обработка дельтового файла остатков: {FilePath}", request.FilePath);
                await _syncService.ProcessStockDeltaFileAsync(request.FilePath);
                
                return Ok(new { message = "Дельтовый файл остатков обработан успешно" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке дельтового файла остатков");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получить актуальные цены и остатки
        /// </summary>
        [HttpGet("current-data")]
        public ActionResult GetCurrentData()
        {
            try
            {
                // Здесь можно добавить логику для получения актуальных данных
                // из базы данных или JSON файлов
                return Ok(new { message = "Актуальные данные получены" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении актуальных данных");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }

    /// <summary>
    /// Запрос на обработку дельтового файла
    /// </summary>
    public class ProcessDeltaRequest
    {
        public string FilePath { get; set; } = string.Empty;
    }
}
