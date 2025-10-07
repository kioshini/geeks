using Microsoft.AspNetCore.Mvc;
using TMKMiniApp.Services;

namespace TMKMiniApp.Controllers
{
    /// <summary>
    /// Контроллер для тестирования загрузки и десериализации JSON-данных
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class JsonDataController : ControllerBase
    {
        private readonly JsonDataService _jsonDataService;

        public JsonDataController(JsonDataService jsonDataService)
        {
            _jsonDataService = jsonDataService;
        }

        /// <summary>
        /// Проверяет корректность десериализации всех JSON-файлов
        /// </summary>
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateJsonFiles()
        {
            var results = await _jsonDataService.ValidateJsonFilesAsync();
            return Ok(new { 
                message = "Проверка десериализации JSON-файлов завершена",
                results = results,
                allValid = results.Values.All(x => x)
            });
        }

        /// <summary>
        /// Загружает номенклатуру
        /// </summary>
        [HttpGet("nomenclature")]
        public async Task<IActionResult> GetNomenclature()
        {
            try
            {
                var nomenclature = await _jsonDataService.LoadNomenclatureAsync();
                return Ok(new { 
                    message = "Номенклатура загружена успешно",
                    count = nomenclature.ArrayOfNomenclatureEl?.Count ?? 0,
                    firstItem = nomenclature.ArrayOfNomenclatureEl?.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Загружает цены
        /// </summary>
        [HttpGet("prices")]
        public async Task<IActionResult> GetPrices()
        {
            try
            {
                var prices = await _jsonDataService.LoadPricesAsync();
                return Ok(new { 
                    message = "Цены загружены успешно",
                    count = prices.ArrayOfPricesEl?.Count ?? 0,
                    firstItem = prices.ArrayOfPricesEl?.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Загружает остатки
        /// </summary>
        [HttpGet("remnants")]
        public async Task<IActionResult> GetRemnants()
        {
            try
            {
                var remnants = await _jsonDataService.LoadRemnantsAsync();
                return Ok(new { 
                    message = "Остатки загружены успешно",
                    count = remnants.ArrayOfRemnantsEl?.Count ?? 0,
                    firstItem = remnants.ArrayOfRemnantsEl?.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Загружает склады
        /// </summary>
        [HttpGet("stocks")]
        public async Task<IActionResult> GetStocks()
        {
            try
            {
                var stocks = await _jsonDataService.LoadStocksAsync();
                return Ok(new { 
                    message = "Склады загружены успешно",
                    count = stocks.ArrayOfStockEl?.Count ?? 0,
                    firstItem = stocks.ArrayOfStockEl?.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Загружает типы
        /// </summary>
        [HttpGet("types")]
        public async Task<IActionResult> GetTypes()
        {
            try
            {
                var types = await _jsonDataService.LoadTypesAsync();
                return Ok(new { 
                    message = "Типы загружены успешно",
                    count = types.ArrayOfTypeEl?.Count ?? 0,
                    firstItem = types.ArrayOfTypeEl?.FirstOrDefault()
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
