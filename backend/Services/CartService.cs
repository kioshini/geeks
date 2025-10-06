using TMKMiniApp.Models;
using TMKMiniApp.Models.DTOs;

namespace TMKMiniApp.Services
{
    public class CartService : ICartService
    {
        private readonly List<CartItem> _cartItems;
        private readonly IProductService _productService;
        private int _nextCartItemId = 1;

        public CartService(IProductService productService)
        {
            _cartItems = new List<CartItem>();
            _productService = productService;
        }

        public async Task<CartDto> GetCartAsync(long userId)
        {
            var userCartItems = _cartItems.Where(ci => ci.UserId == userId).ToList();
            var cartItemDtos = new List<CartItemDto>();

            foreach (var item in userCartItems)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                cartItemDtos.Add(new CartItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    Product = product,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    TotalPrice = item.TotalPrice
                });
            }

            return new CartDto
            {
                UserId = userId,
                Items = cartItemDtos,
                TotalPrice = cartItemDtos.Sum(i => i.TotalPrice),
                TotalItems = cartItemDtos.Sum(i => i.Quantity)
            };
        }

        public async Task<CartItemDto> AddToCartAsync(long userId, AddToCartDto addToCartDto)
        {
            var product = await _productService.GetProductByIdAsync(addToCartDto.ProductId);
            if (product == null)
                throw new ArgumentException("Товар не найден");

            if (!product.IsAvailable)
                throw new InvalidOperationException("Товар недоступен");

            if (addToCartDto.Quantity > product.StockQuantity)
                throw new InvalidOperationException("Недостаточно товара на складе");

            // Проверяем, есть ли уже такой товар в корзине
            var existingItem = _cartItems.FirstOrDefault(ci => ci.UserId == userId && ci.ProductId == addToCartDto.ProductId);
            
            if (existingItem != null)
            {
                existingItem.Quantity += addToCartDto.Quantity;
                existingItem.UpdatedAt = DateTime.UtcNow;
                
                return new CartItemDto
                {
                    Id = existingItem.Id,
                    ProductId = existingItem.ProductId,
                    Product = product,
                    Quantity = existingItem.Quantity,
                    Price = existingItem.Price,
                    TotalPrice = existingItem.TotalPrice
                };
            }
            else
            {
                var cartItem = new CartItem
                {
                    Id = _nextCartItemId++,
                    UserId = userId,
                    ProductId = addToCartDto.ProductId,
                    Quantity = addToCartDto.Quantity,
                    Price = product.Price,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _cartItems.Add(cartItem);

                return new CartItemDto
                {
                    Id = cartItem.Id,
                    ProductId = cartItem.ProductId,
                    Product = product,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price,
                    TotalPrice = cartItem.TotalPrice
                };
            }
        }

        public async Task<CartItemDto?> UpdateCartItemAsync(long userId, int itemId, UpdateCartItemDto updateCartItemDto)
        {
            var cartItem = _cartItems.FirstOrDefault(ci => ci.Id == itemId && ci.UserId == userId);
            if (cartItem == null) return null;

            var product = await _productService.GetProductByIdAsync(cartItem.ProductId);
            if (product == null) return null;

            if (updateCartItemDto.Quantity > product.StockQuantity)
                throw new InvalidOperationException("Недостаточно товара на складе");

            cartItem.Quantity = updateCartItemDto.Quantity;
            cartItem.UpdatedAt = DateTime.UtcNow;

            return new CartItemDto
            {
                Id = cartItem.Id,
                ProductId = cartItem.ProductId,
                Product = product,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price,
                TotalPrice = cartItem.TotalPrice
            };
        }

        public async Task<bool> RemoveFromCartAsync(long userId, int itemId)
        {
            var cartItem = _cartItems.FirstOrDefault(ci => ci.Id == itemId && ci.UserId == userId);
            if (cartItem == null) return false;

            _cartItems.Remove(cartItem);
            return await Task.FromResult(true);
        }

        public async Task<bool> ClearCartAsync(long userId)
        {
            var userCartItems = _cartItems.Where(ci => ci.UserId == userId).ToList();
            foreach (var item in userCartItems)
            {
                _cartItems.Remove(item);
            }
            return await Task.FromResult(true);
        }

        public async Task<bool> ItemExistsInCartAsync(long userId, int productId)
        {
            var exists = _cartItems.Any(ci => ci.UserId == userId && ci.ProductId == productId);
            return await Task.FromResult(exists);
        }

        public async Task<int?> GetCartItemIdByProductIdAsync(long userId, int productId)
        {
            var cartItem = _cartItems.FirstOrDefault(ci => ci.UserId == userId && ci.ProductId == productId);
            return await Task.FromResult(cartItem?.Id);
        }
    }
}