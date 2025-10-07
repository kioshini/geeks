using TMKMiniApp.Models.DTOs;
using TMKMiniApp.Models.OrderModels;

namespace TMKMiniApp.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersByUserIdAsync(long userId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<OrderDto> CreateOrderFromRequestAsync(OrderRequest orderRequest);
        Task<OrderDto?> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto updateOrderStatusDto);
        Task<bool> DeleteOrderAsync(int orderId);
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<IEnumerable<OrderDto>> GetOrdersByStatusAsync(string status);
    }
}
