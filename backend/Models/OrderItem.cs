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
        [JsonPropertyName("ID")]
        public string ID { get; set; } = string.Empty; // ID - ссылка на Nomenclature.ID (string)
        
        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty; // ОБЯЗАТЕЛЬНО: при формировании заказа заполнить из каталога
        
        [Required]
        [JsonPropertyName("Quantity")]
        public double Quantity { get; set; }
        
        [Required]
        [JsonPropertyName("Unit")]
        public string Unit { get; set; } = string.Empty; // "m" или "t"
        
        [Required]
        [JsonPropertyName("Price")]
        public decimal Price { get; set; } // цена за единицу в выбранной единице
        
        [JsonPropertyName("TotalPrice")]
        public decimal TotalPrice => Price * (decimal)Quantity;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
