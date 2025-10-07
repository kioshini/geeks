using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TMKMiniApp.Models
{
    /// <summary>
    /// Модель товара в заказе
    /// Соответствует официальному описанию JSON-структуры
    /// </summary>
    public class OrderItem
    {
        public int Id { get; set; }
        
        [Required]
        [JsonPropertyName("ProductId")]
        public string ProductId { get; set; } = string.Empty;
        
        public Product? Product { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        [JsonPropertyName("Quantity")]
        public int Quantity { get; set; }
        
        [Required]
        [JsonPropertyName("Unit")]
        public string Unit { get; set; } = string.Empty; // "т" или "м"
        
        [Required]
        [JsonPropertyName("UnitPrice")]
        public decimal UnitPrice { get; set; }
        
        [JsonPropertyName("TotalPrice")]
        public decimal TotalPrice => UnitPrice * Quantity;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
