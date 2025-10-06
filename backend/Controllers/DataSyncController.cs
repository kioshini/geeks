using Microsoft.AspNetCore.Mvc;
using TMKMiniApp.Services;

namespace TMKMiniApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataSyncController : ControllerBase
    {
        private readonly IDataSyncService _dataSyncService;
        private readonly ILogger<DataSyncController> _logger;

        public DataSyncController(IDataSyncService dataSyncService, ILogger<DataSyncController> logger)
        {
            _dataSyncService = dataSyncService;
            _logger = logger;
        }

        /// <summary>
        /// Синхронизировать номенклатуру
        /// </summary>
        [HttpPost("nomenclature")]
        public async Task<ActionResult> SyncNomenclature([FromBody] string jsonData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonData))
                    return BadRequest("Данные для синхронизации не могут быть пустыми");

                await _dataSyncService.SyncNomenclatureAsync(jsonData);
                return Ok(new { message = "Номенклатура успешно синхронизирована" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при синхронизации номенклатуры");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Синхронизировать цены
        /// </summary>
        [HttpPost("prices")]
        public async Task<ActionResult> SyncPrices([FromBody] string jsonData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonData))
                    return BadRequest("Данные для синхронизации не могут быть пустыми");

                await _dataSyncService.SyncPricesAsync(jsonData);
                return Ok(new { message = "Цены успешно синхронизированы" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при синхронизации цен");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Синхронизировать остатки
        /// </summary>
        [HttpPost("remnants")]
        public async Task<ActionResult> SyncRemnants([FromBody] string jsonData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonData))
                    return BadRequest("Данные для синхронизации не могут быть пустыми");

                await _dataSyncService.SyncRemnantsAsync(jsonData);
                return Ok(new { message = "Остатки успешно синхронизированы" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при синхронизации остатков");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Синхронизировать склады
        /// </summary>
        [HttpPost("stocks")]
        public async Task<ActionResult> SyncStocks([FromBody] string jsonData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonData))
                    return BadRequest("Данные для синхронизации не могут быть пустыми");

                await _dataSyncService.SyncStocksAsync(jsonData);
                return Ok(new { message = "Склады успешно синхронизированы" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при синхронизации складов");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Синхронизировать типы
        /// </summary>
        [HttpPost("types")]
        public async Task<ActionResult> SyncTypes([FromBody] string jsonData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonData))
                    return BadRequest("Данные для синхронизации не могут быть пустыми");

                await _dataSyncService.SyncTypesAsync(jsonData);
                return Ok(new { message = "Типы успешно синхронизированы" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при синхронизации типов");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Проверить актуальность данных
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult> GetSyncStatus()
        {
            try
            {
                var isUpToDate = await _dataSyncService.IsDataUpToDateAsync();
                var lastSyncTime = await _dataSyncService.GetLastSyncTimeAsync();

                return Ok(new
                {
                    isUpToDate,
                    lastSyncTime,
                    message = isUpToDate ? "Данные актуальны" : "Требуется синхронизация"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке статуса синхронизации");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}
