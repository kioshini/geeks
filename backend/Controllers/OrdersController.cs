using Microsoft.AspNetCore.Mvc;
using TMKMiniApp.Models.DTOs;
using TMKMiniApp.Models.OrderModels;
using TMKMiniApp.Services;
using TMKMiniApp.Validators;
using FluentValidation;

namespace TMKMiniApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;
        private readonly OrderRequestValidator _orderRequestValidator;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger, OrderRequestValidator orderRequestValidator)
        {
            _orderService = orderService;
            _logger = logger;
            _orderRequestValidator = orderRequestValidator;
        }

        /// <summary>
        /// Получить все заказы (для администратора)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех заказов");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получить заказы пользователя
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders(long userId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении заказов пользователя {UserId}", userId);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получить заказ по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound($"Заказ с ID {id} не найден");

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении заказа с ID {OrderId}", id);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Создать новый заказ с валидацией
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderRequest orderRequest)
        {
            try
            {
                // Логируем входящие данные для отладки
                _logger.LogInformation("Получен запрос на создание заказа: INN='{INN}', FirstName='{FirstName}', LastName='{LastName}', ItemsCount={ItemsCount}", 
                    orderRequest.INN, orderRequest.FirstName, orderRequest.LastName, orderRequest.OrderedItems?.Count ?? 0);
                
                if (orderRequest.OrderedItems != null && orderRequest.OrderedItems.Any())
                {
                    _logger.LogInformation("Первый товар: ID='{ID}', Name='{Name}', Quantity={Quantity}, Unit='{Unit}'", 
                        orderRequest.OrderedItems[0].ID, orderRequest.OrderedItems[0].Name, orderRequest.OrderedItems[0].Quantity, orderRequest.OrderedItems[0].Unit);
                }

                // Валидация с помощью FluentValidation
                var validationResult = await _orderRequestValidator.ValidateAsync(orderRequest);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => new { Field = e.PropertyName, Message = e.ErrorMessage });
                    _logger.LogWarning("Ошибки валидации FluentValidation: {Errors}", string.Join(", ", errors.Select(e => $"{e.Field}: {e.Message}")));
                    return BadRequest(new { Message = "Ошибки валидации", Errors = errors });
                }

                // Дополнительная валидация ModelState
                if (!ModelState.IsValid)
                {
                    var modelStateErrors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );
                    _logger.LogWarning("Ошибки валидации ModelState: {Errors}", 
                        string.Join(", ", modelStateErrors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}")));
                    
                    // Возвращаем ошибки в том же формате, что и FluentValidation
                    var errors = modelStateErrors.Select(kvp => new { Field = kvp.Key, Message = string.Join(", ", kvp.Value) });
                    return BadRequest(new { Message = "Ошибки валидации", Errors = errors });
                }

                var order = await _orderService.CreateOrderFromRequestAsync(orderRequest);
                
                // Возвращаем успешный ответ
                return Ok(new { success = true, orderId = order.Id });
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
                _logger.LogError(ex, "Ошибка при создании заказа");
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Обновить статус заказа
        /// </summary>
        [HttpPut("{id}/status")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(int id, UpdateOrderStatusDto updateOrderStatusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var order = await _orderService.UpdateOrderStatusAsync(id, updateOrderStatusDto);
                if (order == null)
                    return NotFound($"Заказ с ID {id} не найден");

                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении статуса заказа {OrderId}", id);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Удалить заказ
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);
                if (!result)
                    return NotFound($"Заказ с ID {id} не найден");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении заказа с ID {OrderId}", id);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }

        /// <summary>
        /// Получить заказы по статусу
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByStatus(string status)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatusAsync(status);
                return Ok(orders);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении заказов по статусу {Status}", status);
                return StatusCode(500, "Внутренняя ошибка сервера");
            }
        }
    }
}