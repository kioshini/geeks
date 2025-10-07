using System.ComponentModel.DataAnnotations;

namespace TMKMiniApp.Models.DTOs
{
    public class OrderDto
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? INN { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    public class OrderItemDto
    {
        public int Id { get; set; }
        public string ProductId { get; set; } = string.Empty;
        public ProductDto? Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
    }
    
    public class CreateOrderDto
    {
        [Required]
        public long UserId { get; set; }
        
        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;
        
        public string? INN { get; set; }
        
        [Required]
        public string Phone { get; set; } = string.Empty;
        
        public string? Email { get; set; }
        
        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
        
        public string? Notes { get; set; }
    }
    
    public class CreateOrderItemDto
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public int Quantity { get; set; }
    }
    
    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty;
        
        public string? Notes { get; set; }
    }
}
