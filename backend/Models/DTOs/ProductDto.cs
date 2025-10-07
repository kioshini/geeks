using System.ComponentModel.DataAnnotations;

namespace TMKMiniApp.Models.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public decimal? Diameter { get; set; }
        public decimal? Length { get; set; }
        public decimal? Thickness { get; set; }
        public string? Unit { get; set; }
        public decimal Price { get; set; }
        public decimal PriceT { get; set; } // Цена за тонну
        public decimal PriceM { get; set; } // Цена за метр
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
        
        // Discount information
        public decimal? VolumeDiscountThreshold { get; set; }
        public decimal? VolumeDiscountPercent { get; set; }
        public decimal? MaterialDiscountPercent { get; set; }
        public decimal? FinalPrice { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? TotalDiscountPercent { get; set; }
        public bool HasVolumeDiscount { get; set; }
        public bool HasMaterialDiscount { get; set; }
    }
    
    public class CreateProductDto
    {
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
        
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Цена должна быть больше или равна 0")]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Количество должно быть больше или равно 0")]
        public int StockQuantity { get; set; }
    }
    
    public class UpdateProductDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? Material { get; set; }
        public decimal? Diameter { get; set; }
        public decimal? Length { get; set; }
        public decimal? Thickness { get; set; }
        public string? Unit { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
    }
}
