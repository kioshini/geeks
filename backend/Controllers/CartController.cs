using Microsoft.AspNetCore.Mvc;
using TMKMiniApp.Models;
using TMKMiniApp.Models.DTOs;
using TMKMiniApp.Services;

namespace TMKMiniApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IDiscountService _discountService;
        private readonly IProductService _productService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, IDiscountService discountService, IProductService productService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _discountService = discountService;
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Получить корзину пользователя
        /// </summary>
        [HttpGet("{userId}")]
        public async Task<ActionResult<CartDto>> GetCart(long userId)
        {
            try
            {
                var cart = await _cartService.GetCartAsync(userId);
                return Ok(cart);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении корзины пользователя {UserId}", userId);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Добавить товар в корзину
        /// </summary>
        [HttpPost("{userId}/items")]
        public async Task<ActionResult<CartItemDto>> AddToCart(long userId, AddToCartDto addToCartDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var cartItem = await _cartService.AddToCartAsync(userId, addToCartDto);
                return Ok(cartItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении товара в корзину пользователя {UserId}", userId);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Обновить количество товара в корзине
        /// </summary>
        [HttpPut("{userId}/items/{itemId}")]
        public async Task<ActionResult<CartItemDto>> UpdateCartItem(long userId, int itemId, UpdateCartItemDto updateCartItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var cartItem = await _cartService.UpdateCartItemAsync(userId, itemId, updateCartItemDto);
                if (cartItem == null)
                    return NotFound($"Товар с ID {itemId} не найден в корзине пользователя {userId}");

                return Ok(cartItem);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении товара в корзине пользователя {UserId}", userId);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Удалить товар из корзины
        /// </summary>
        [HttpDelete("{userId}/items/{itemId}")]
        public async Task<ActionResult> RemoveFromCart(long userId, int itemId)
        {
            try
            {
                var result = await _cartService.RemoveFromCartAsync(userId, itemId);
                if (!result)
                    return NotFound($"Товар с ID {itemId} не найден в корзине пользователя {userId}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении товара из корзины пользователя {UserId}", userId);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Очистить корзину пользователя
        /// </summary>
        [HttpDelete("{userId}")]
        public async Task<ActionResult> ClearCart(long userId)
        {
            try
            {
                await _cartService.ClearCartAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при очистке корзины пользователя {UserId}", userId);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Проверить, есть ли товар в корзине
        /// </summary>
        [HttpGet("{userId}/items/check")]
        public async Task<ActionResult<bool>> CheckItemInCart(long userId, [FromQuery] int productId)
        {
            try
            {
                var exists = await _cartService.ItemExistsInCartAsync(userId, productId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке товара в корзине пользователя {UserId}", userId);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }



        /// <summary>
        /// Рассчитать скидку для товара
        /// </summary>
        [HttpPost("calculate-discount")]
        public async Task<ActionResult<DiscountInfo>> CalculateDiscount([FromBody] CalculateDiscountRequest request)
        {
            try
            {
                _logger.LogInformation("Расчет скидки для товара {ProductId}, количество: {Quantity}, единица: {Unit}", 
                    request.ProductId, request.Quantity, request.Unit);

                // Получаем товар
                var productDto = await _productService.GetProductByIdAsync(int.Parse(request.ProductId));
                if (productDto == null)
                {
                    return NotFound($"Товар с ID {request.ProductId} не найден");
                }

                // Создаем объект Product из DTO
                var product = new Product
                {
                    Id = productDto.Id,
                    Name = productDto.Name,
                    Code = productDto.Code,
                    Description = productDto.Description,
                    Type = productDto.Type,
                    Material = productDto.Material,
                    Diameter = productDto.Diameter,
                    Length = productDto.Length,
                    Thickness = productDto.Thickness,
                    Unit = productDto.Unit,
                    Price = productDto.Price,
                    StockQuantity = productDto.StockQuantity,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Получаем данные о ценах
                var priceData = await _productService.GetPriceDataAsync(request.ProductId);
                
                // Рассчитываем скидку
                var discountInfo = _discountService.CalculateDiscount(product, request.Quantity, request.Unit, priceData);
                
                _logger.LogInformation("Скидка рассчитана: {DiscountPercent}%, финальная цена: {FinalPrice}", 
                    discountInfo.TotalDiscountPercent, discountInfo.FinalPrice);

                return Ok(discountInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при расчете скидки для товара {ProductId}", request.ProductId);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Обновить количество товара в корзине по ID товара
        /// </summary>
        [HttpPut("{userId}/items/product/{productId}")]
        public async Task<ActionResult<CartItemDto>> UpdateCartItemByProductId(long userId, string productId, UpdateCartItemDto updateCartItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var cartItem = await _cartService.UpdateCartItemByProductIdAsync(userId, productId, updateCartItemDto);
                if (cartItem == null)
                    return NotFound($"Товар с ID {productId} не найден в корзине пользователя {userId}");

                return Ok(cartItem);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении товара в корзине пользователя {UserId}", userId);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Удалить товар из корзины по ID товара
        /// </summary>
        [HttpDelete("{userId}/items/product/{productId}")]
        public async Task<ActionResult> RemoveFromCartByProductId(long userId, string productId)
        {
            try
            {
                var result = await _cartService.RemoveFromCartByProductIdAsync(userId, productId);
                if (!result)
                    return NotFound($"Товар с ID {productId} не найден в корзине пользователя {userId}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении товара из корзины пользователя {UserId}", userId);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }

    /// <summary>
    /// Запрос на расчет скидки
    /// </summary>
    public class CalculateDiscountRequest
    {
        public string ProductId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public string Unit { get; set; } = "шт";
    }
}