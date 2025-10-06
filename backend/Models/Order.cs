using System.ComponentModel.DataAnnotations;

namespace TMKMiniApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        
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
        
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        
        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public string? Notes { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Processing = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5
    }
}
