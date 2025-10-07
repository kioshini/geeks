using System.ComponentModel.DataAnnotations;

namespace TMKMiniApp.Models.DTOs
{
    public class CartDto
    {
        public long UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalPrice { get; set; }
        public int TotalItems { get; set; }
    }
    
    public class CartItemDto
    {
        public int Id { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public ProductDto? Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string Unit { get; set; } = "шт";
    }
    
    public class AddToCartDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public int Quantity { get; set; }
        
        public string? Unit { get; set; }
    }
    
    public class UpdateCartItemDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public int Quantity { get; set; }
        
        public string? Unit { get; set; }
    }
}
