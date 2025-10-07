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
                    ProductId = item.ProductId.ToString(),
                    Product = product,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    TotalPrice = item.TotalPrice,
                    Unit = item.Unit
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
            if (!int.TryParse(addToCartDto.ProductId, out int productId))
                throw new ArgumentException("Неверный формат ID товара");
                
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                throw new ArgumentException("Товар не найден");

            if (!product.IsAvailable)
                throw new InvalidOperationException("Товар недоступен");

            if (addToCartDto.Quantity > product.StockQuantity)
                throw new InvalidOperationException("Недостаточно товара на складе");

            // Проверяем, есть ли уже такой товар в корзине
            var existingItem = _cartItems.FirstOrDefault(ci => ci.UserId == userId && ci.ProductId == productId);
            
            if (existingItem != null)
            {
                var unit = addToCartDto.Unit ?? "т";
                
                // Если единица измерения совпадает, увеличиваем количество
                if (existingItem.Unit == unit)
                {
                    existingItem.Quantity += addToCartDto.Quantity;
                    existingItem.UpdatedAt = DateTime.UtcNow;
                    
                    return new CartItemDto
                    {
                        Id = existingItem.Id,
                        ProductId = existingItem.ProductId.ToString(),
                        Product = product,
                        Quantity = existingItem.Quantity,
                        Price = existingItem.Price,
                        TotalPrice = existingItem.TotalPrice,
                        Unit = existingItem.Unit
                    };
                }
                else
                {
                    // Если единица измерения отличается, создаем новый элемент
                    var price = unit == "м" ? product.PriceM : product.PriceT;
                    
                    var newCartItem = new CartItem
                    {
                        Id = _nextCartItemId++,
                        UserId = userId,
                        ProductId = productId,
                        Quantity = addToCartDto.Quantity,
                        Price = price,
                        Unit = unit,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _cartItems.Add(newCartItem);

                    return new CartItemDto
                    {
                        Id = newCartItem.Id,
                        ProductId = newCartItem.ProductId.ToString(),
                        Product = product,
                        Quantity = newCartItem.Quantity,
                        Price = newCartItem.Price,
                        TotalPrice = newCartItem.TotalPrice,
                        Unit = unit
                    };
                }
            }
            else
            {
                // Определяем цену в зависимости от единицы измерения
                var unit = addToCartDto.Unit ?? "т";
                var price = unit == "м" ? product.PriceM : product.PriceT;
                
                var cartItem = new CartItem
                {
                    Id = _nextCartItemId++,
                    UserId = userId,
                    ProductId = productId,
                    Quantity = addToCartDto.Quantity,
                    Price = price,
                    Unit = unit,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _cartItems.Add(cartItem);

                return new CartItemDto
                {
                    Id = cartItem.Id,
                    ProductId = cartItem.ProductId.ToString(),
                    Product = product,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Price,
                    TotalPrice = cartItem.TotalPrice,
                    Unit = unit
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
            
            // Обновляем цену и единицу измерения, если они изменились
            if (!string.IsNullOrEmpty(updateCartItemDto.Unit) && updateCartItemDto.Unit != cartItem.Unit)
            {
                cartItem.Unit = updateCartItemDto.Unit;
                cartItem.Price = updateCartItemDto.Unit == "м" ? product.PriceM : product.PriceT;
            }
            
            cartItem.UpdatedAt = DateTime.UtcNow;

            return new CartItemDto
            {
                Id = cartItem.Id,
                ProductId = cartItem.ProductId.ToString(),
                Product = product,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price,
                TotalPrice = cartItem.TotalPrice,
                Unit = cartItem.Unit
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

        public async Task<CartItemDto?> UpdateCartItemByProductIdAsync(long userId, string productId, UpdateCartItemDto updateCartItemDto)
        {
            if (!int.TryParse(productId, out int productIdInt))
                throw new ArgumentException("Неверный формат ID товара");
                
            var cartItem = _cartItems.FirstOrDefault(ci => ci.UserId == userId && ci.ProductId == productIdInt);
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
                ProductId = cartItem.ProductId.ToString(),
                Product = product,
                Quantity = cartItem.Quantity,
                Price = cartItem.Price,
                TotalPrice = cartItem.TotalPrice,
                Unit = cartItem.Unit
            };
        }

        public async Task<bool> RemoveFromCartByProductIdAsync(long userId, string productId)
        {
            if (!int.TryParse(productId, out int productIdInt))
                throw new ArgumentException("Неверный формат ID товара");
                
            var cartItem = _cartItems.FirstOrDefault(ci => ci.UserId == userId && ci.ProductId == productIdInt);
            if (cartItem == null) return false;

            _cartItems.Remove(cartItem);
            return await Task.FromResult(true);
        }
    }
}