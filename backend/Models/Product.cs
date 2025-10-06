using System.ComponentModel.DataAnnotations;

namespace TMKMiniApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Code { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [Required]
        public string Type { get; set; } = string.Empty;
        
        [Required]
        public string Material { get; set; } = string.Empty;
        
        public decimal? Diameter { get; set; }
        
        public decimal? Length { get; set; }
        
        public decimal? Thickness { get; set; }
        
        public string? Unit { get; set; } = "шт";
        
        public decimal Price { get; set; }
        
        public int StockQuantity { get; set; }
        
        public bool IsAvailable => StockQuantity > 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Discount properties
        public decimal? VolumeDiscountThreshold { get; set; } // e.g., 100 meters
        public decimal? VolumeDiscountPercent { get; set; } // e.g., 5%
        public decimal? MaterialDiscountPercent { get; set; } // e.g., 3% for stainless steel
    }
}
