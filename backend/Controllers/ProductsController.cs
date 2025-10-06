using Microsoft.AspNetCore.Mvc;
using TMKMiniApp.Models;
using TMKMiniApp.Models.DTOs;
using TMKMiniApp.Services;

namespace TMKMiniApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Получить все товары
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка товаров");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получить товар по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound($"Товар с ID {id} не найден");

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении товара с ID {ProductId}", id);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получить товары по типу
        /// </summary>
        [HttpGet("type/{type}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByType(string type)
        {
            try
            {
                var products = await _productService.GetProductsByTypeAsync(type);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении товаров по типу {Type}", type);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Поиск товаров
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string q)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                    return BadRequest("Параметр поиска не может быть пустым");

                var products = await _productService.SearchProductsAsync(q);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске товаров по запросу {Query}", q);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Создать новый товар
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _productService.CreateProductAsync(createProductDto);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании товара");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Обновить товар
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var product = await _productService.UpdateProductAsync(id, updateProductDto);
                if (product == null)
                    return NotFound($"Товар с ID {id} не найден");

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении товара с ID {ProductId}", id);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Удалить товар
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                    return NotFound($"Товар с ID {id} не найден");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении товара с ID {ProductId}", id);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Обновить остатки товара
        /// </summary>
        [HttpPut("{id}/stock")]
        public async Task<ActionResult> UpdateStock(int id, [FromBody] int quantity)
        {
            try
            {
                if (quantity < 0)
                    return BadRequest("Количество не может быть отрицательным");

                var result = await _productService.UpdateStockAsync(id, quantity);
                if (!result)
                    return NotFound($"Товар с ID {id} не найден");

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении остатков товара с ID {ProductId}", id);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получить типы товаров
        /// </summary>
        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetProductTypes()
        {
            try
            {
                var types = await _productService.GetProductTypesAsync();
                return Ok(types);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении типов товаров");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получить категории товаров (алиас для types)
        /// </summary>
        [HttpGet("categories")]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetCategories()
        {
            try
            {
                var types = await _productService.GetProductTypesAsync();
                return Ok(types);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении категорий товаров");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}
