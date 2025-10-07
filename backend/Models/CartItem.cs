using System.ComponentModel.DataAnnotations;

namespace TMKMiniApp.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        
        [Required]
        public long UserId { get; set; }
        
        [Required]
        public int ProductId { get; set; }
        
        public Product? Product { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        public int Quantity { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public string Unit { get; set; } = "т";
        
        public decimal TotalPrice => Price * Quantity;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
