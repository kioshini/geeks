using Microsoft.AspNetCore.Mvc;
using TMKMiniApp.Services;

namespace TMKMiniApp.Controllers
{
    /// <summary>
    /// Контроллер для управления динамическими обновлениями цен и остатков
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DeltaUpdatesController : ControllerBase
    {
        private readonly IDynamicDeltaUpdatesService _deltaUpdatesService;
        private readonly ILogger<DeltaUpdatesController> _logger;

        public DeltaUpdatesController(
            IDynamicDeltaUpdatesService deltaUpdatesService,
            ILogger<DeltaUpdatesController> logger)
        {
            _deltaUpdatesService = deltaUpdatesService;
            _logger = logger;
        }

        /// <summary>
        /// Запускает мониторинг папки updates
        /// </summary>
        [HttpPost("start-monitoring")]
        public async Task<IActionResult> StartMonitoring()
        {
            try
            {
                await _deltaUpdatesService.StartMonitoringAsync();
                return Ok(new { Message = "Мониторинг папки updates запущен" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запуске мониторинга");
                return StatusCode(500, new { Error = "Ошибка при запуске мониторинга" });
            }
        }

        /// <summary>
        /// Останавливает мониторинг папки updates
        /// </summary>
        [HttpPost("stop-monitoring")]
        public async Task<IActionResult> StopMonitoring()
        {
            try
            {
                await _deltaUpdatesService.StopMonitoringAsync();
                return Ok(new { Message = "Мониторинг папки updates остановлен" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при остановке мониторинга");
                return StatusCode(500, new { Error = "Ошибка при остановке мониторинга" });
            }
        }

        /// <summary>
        /// Обрабатывает файл с дельтами цен вручную
        /// </summary>
        [HttpPost("process-prices-file")]
        public async Task<IActionResult> ProcessPricesFile([FromBody] ProcessFileRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FilePath))
                {
                    return BadRequest(new { Error = "Путь к файлу не указан" });
                }

                await _deltaUpdatesService.ProcessPricesDeltaFileAsync(request.FilePath);
                return Ok(new { Message = $"Файл цен {request.FilePath} обработан" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке файла цен: {FilePath}", request.FilePath);
                return StatusCode(500, new { Error = "Ошибка при обработке файла цен" });
            }
        }

        /// <summary>
        /// Обрабатывает файл с дельтами остатков вручную
        /// </summary>
        [HttpPost("process-remnants-file")]
        public async Task<IActionResult> ProcessRemnantsFile([FromBody] ProcessFileRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.FilePath))
                {
                    return BadRequest(new { Error = "Путь к файлу не указан" });
                }

                await _deltaUpdatesService.ProcessRemnantsDeltaFileAsync(request.FilePath);
                return Ok(new { Message = $"Файл остатков {request.FilePath} обработан" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке файла остатков: {FilePath}", request.FilePath);
                return StatusCode(500, new { Error = "Ошибка при обработке файла остатков" });
            }
        }

        /// <summary>
        /// Тестирует применение дельты к цене
        /// </summary>
        [HttpPost("test-price-delta")]
        public IActionResult TestPriceDelta([FromBody] TestDeltaRequest request)
        {
            try
            {
                var result = _deltaUpdatesService.ApplyPriceDelta(request.CurrentValue, request.Delta);
                return Ok(new 
                { 
                    CurrentValue = request.CurrentValue,
                    Delta = request.Delta,
                    NewValue = result,
                    Message = "Дельта цены применена успешно"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при тестировании дельты цены");
                return StatusCode(500, new { Error = "Ошибка при тестировании дельты цены" });
            }
        }

        /// <summary>
        /// Тестирует применение дельты к остатку
        /// </summary>
        [HttpPost("test-stock-delta")]
        public IActionResult TestStockDelta([FromBody] TestDeltaRequest request)
        {
            try
            {
                var result = _deltaUpdatesService.ApplyStockDelta(request.CurrentValue, request.Delta);
                return Ok(new 
                { 
                    CurrentValue = request.CurrentValue,
                    Delta = request.Delta,
                    NewValue = result,
                    Message = "Дельта остатка применена успешно"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при тестировании дельты остатка");
                return StatusCode(500, new { Error = "Ошибка при тестировании дельты остатка" });
            }
        }
    }

    /// <summary>
    /// Запрос на обработку файла
    /// </summary>
    public class ProcessFileRequest
    {
        public string FilePath { get; set; } = string.Empty;
    }

    /// <summary>
    /// Запрос на тестирование дельты
    /// </summary>
    public class TestDeltaRequest
    {
        public decimal CurrentValue { get; set; }
        public decimal Delta { get; set; }
    }
}
