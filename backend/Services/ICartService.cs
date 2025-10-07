using TMKMiniApp.Models.DTOs;

namespace TMKMiniApp.Services
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(long userId);
        Task<CartItemDto> AddToCartAsync(long userId, AddToCartDto addToCartDto);
        Task<CartItemDto?> UpdateCartItemAsync(long userId, int itemId, UpdateCartItemDto updateCartItemDto);
        Task<CartItemDto?> UpdateCartItemByProductIdAsync(long userId, string productId, UpdateCartItemDto updateCartItemDto);
        Task<bool> RemoveFromCartAsync(long userId, int itemId);
        Task<bool> RemoveFromCartByProductIdAsync(long userId, string productId);
        Task<bool> ClearCartAsync(long userId);
        Task<bool> ItemExistsInCartAsync(long userId, int productId);
        Task<int?> GetCartItemIdByProductIdAsync(long userId, int productId);
    }
}
