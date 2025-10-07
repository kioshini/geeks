using TMKMiniApp.Models;
using TMKMiniApp.Models.DTOs;
using TMKMiniApp.Models.OrderModels;

namespace TMKMiniApp.Services
{
    public class OrderService : IOrderService
    {
        private readonly List<Order> _orders;
        private readonly IProductService _productService;
        private readonly ITelegramService _telegramService;
        private int _nextOrderId = 1;

        public OrderService(IProductService productService, ITelegramService telegramService)
        {
            _orders = new List<Order>();
            _productService = productService;
            _telegramService = telegramService;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(long userId)
        {
            var userOrders = _orders.Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedAt);
            var orderDtos = new List<OrderDto>();

            foreach (var order in userOrders)
            {
                orderDtos.Add(await MapToDtoAsync(order));
            }

            return orderDtos;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            return order != null ? await MapToDtoAsync(order) : null;
        }

        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            var orderItems = new List<OrderItem>();
            var orderItemId = 1;

            foreach (var itemDto in createOrderDto.Items)
            {
                var product = await _productService.GetProductByIdAsync(int.Parse(itemDto.ProductId));
                
                if (product == null)
                    throw new ArgumentException($"Товар с ID {itemDto.ProductId} не найден");

                if (!product.IsAvailable)
                    throw new InvalidOperationException($"Товар {product.Name} недоступен");

                if (itemDto.Quantity > product.StockQuantity)
                    throw new InvalidOperationException($"Недостаточно товара {product.Name} на складе");

                var orderItem = new OrderItem
                {
                    ID = itemDto.ProductId,
                    Name = product.Name,
                    Quantity = itemDto.Quantity,
                    Price = product.Price,
                    Unit = "шт", // По умолчанию
                    CreatedAt = DateTime.UtcNow
                };

                orderItems.Add(orderItem);

                // Обновляем остатки на складе
                await _productService.UpdateStockAsync(int.Parse(itemDto.ProductId), product.StockQuantity - itemDto.Quantity);
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = createOrderDto.UserId,
                FirstName = createOrderDto.FirstName,
                LastName = createOrderDto.LastName,
                INN = createOrderDto.INN,
                Phone = createOrderDto.Phone,
                Email = createOrderDto.Email,
                Items = orderItems,
                Status = OrderStatus.Pending,
                Notes = createOrderDto.Notes,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _orders.Add(order);
            
            // Отправляем заказ в Telegram
            try
            {
                await _telegramService.SendOrderAsync(order);
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем создание заказа
                Console.WriteLine($"Ошибка при отправке заказа в Telegram: {ex.Message}");
            }
            
            return await MapToDtoAsync(order);
        }

        public async Task<OrderDto> CreateOrderFromRequestAsync(OrderRequest orderRequest)
        {
            var orderItems = new List<OrderItem>();
            var orderItemId = 1;

            foreach (var itemDto in orderRequest.OrderedItems)
            {
                var product = await _productService.GetProductByIdAsync(int.Parse(itemDto.ID));
                if (product == null)
                    throw new ArgumentException($"Товар с ID {itemDto.ID} не найден");

                if (!product.IsAvailable)
                    throw new InvalidOperationException($"Товар {product.Name} недоступен");

                if (itemDto.Quantity > product.StockQuantity)
                    throw new InvalidOperationException($"Недостаточно товара {product.Name} на складе");

                var orderItem = new OrderItem
                {
                    Id = orderItemId++,
                    ID = itemDto.ID,
                    Name = itemDto.Name ?? product.Name ?? $"[ID:{itemDto.ID}] (название не найдено)",
                    Quantity = itemDto.Quantity,
                    Unit = itemDto.Unit,
                    Price = itemDto.Price,
                    CreatedAt = DateTime.UtcNow
                };

                orderItems.Add(orderItem);

                // Обновляем остатки на складе
                await _productService.UpdateStockAsync(int.Parse(itemDto.ID), product.StockQuantity - (int)itemDto.Quantity);
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = 1, // Временное значение, в реальном приложении получать из контекста пользователя
                FirstName = orderRequest.FirstName,
                LastName = orderRequest.LastName,
                INN = orderRequest.INN,
                Phone = orderRequest.Phone,
                Email = orderRequest.Email,
                Comment = orderRequest.Comment,
                Items = orderItems,
                TotalPrice = orderItems.Sum(x => x.Price * (decimal)x.Quantity),
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _orders.Add(order);
            
            // Отправляем заказ в Telegram
            try
            {
                await _telegramService.SendOrderAsync(order);
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем создание заказа
                Console.WriteLine($"Ошибка при отправке заказа в Telegram: {ex.Message}");
            }
            
            return await MapToDtoAsync(order);
        }

        public async Task<OrderDto?> UpdateOrderStatusAsync(Guid orderId, UpdateOrderStatusDto updateOrderStatusDto)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return null;

            if (Enum.TryParse<OrderStatus>(updateOrderStatusDto.Status, true, out var status))
            {
                order.Status = status;
            }
            else
            {
                throw new ArgumentException("Недопустимый статус заказа");
            }

            if (updateOrderStatusDto.Notes != null)
            {
                order.Notes = updateOrderStatusDto.Notes;
            }

            order.UpdatedAt = DateTime.UtcNow;

            return await MapToDtoAsync(order);
        }

        public async Task<bool> DeleteOrderAsync(Guid orderId)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return false;

            _orders.Remove(order);
            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync()
        {
            var orderDtos = new List<OrderDto>();
            foreach (var order in _orders.OrderByDescending(o => o.CreatedAt))
            {
                orderDtos.Add(await MapToDtoAsync(order));
            }
            return orderDtos;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status)
        {
            if (!Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            {
                throw new ArgumentException("Недопустимый статус заказа");
            }

            var orders = _orders.Where(o => o.Status == orderStatus).OrderByDescending(o => o.CreatedAt);
            var orderDtos = new List<OrderDto>();

            foreach (var order in orders)
            {
                orderDtos.Add(await MapToDtoAsync(order));
            }

            return orderDtos;
        }

        private async Task<OrderDto> MapToDtoAsync(Order order)
        {
            var orderItemDtos = new List<OrderItemDto>();

            foreach (var item in order.Items)
            {
                var product = await _productService.GetProductByIdAsync(int.Parse(item.ID));
                orderItemDtos.Add(new OrderItemDto
                {
                    Id = item.Id.ToString(),
                    ProductId = item.ID,
                    Product = product,
                    Quantity = (int)item.Quantity,
                    Price = item.Price,
                    TotalPrice = item.Price * (decimal)item.Quantity
                });
            }

            return new OrderDto
            {
                Id = order.Id.ToString(),
                UserId = order.UserId,
                FirstName = order.FirstName,
                LastName = order.LastName,
                INN = order.INN,
                Phone = order.Phone,
                Email = order.Email,
                Items = orderItemDtos,
                TotalPrice = order.TotalPrice,
                Status = order.Status.ToString(),
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };
        }
    }
}