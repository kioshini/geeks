using Microsoft.AspNetCore.Mvc;
using TMKMiniApp.Models.DTOs;
using TMKMiniApp.Services;

namespace TMKMiniApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
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
        /// Обновить количество товара в корзине по productId
        /// </summary>
        [HttpPut("{userId}/items/product/{productId}")]
        public async Task<ActionResult<CartItemDto>> UpdateCartItemByProductId(long userId, int productId, UpdateCartItemDto updateCartItemDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var itemId = await _cartService.GetCartItemIdByProductIdAsync(userId, productId);
                if (itemId == null)
                    return NotFound($"Товар с ID {productId} не найден в корзине пользователя {userId}");

                var cartItem = await _cartService.UpdateCartItemAsync(userId, itemId.Value, updateCartItemDto);
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
        /// Удалить товар из корзины по productId
        /// </summary>
        [HttpDelete("{userId}/items/product/{productId}")]
        public async Task<ActionResult> RemoveFromCartByProductId(long userId, int productId)
        {
            try
            {
                var itemId = await _cartService.GetCartItemIdByProductIdAsync(userId, productId);
                if (itemId == null)
                    return NotFound($"Товар с ID {productId} не найден в корзине пользователя {userId}");

                var result = await _cartService.RemoveFromCartAsync(userId, itemId.Value);
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
}