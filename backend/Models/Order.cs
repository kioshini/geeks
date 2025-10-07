using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TMKMiniApp.Models
{
    /// <summary>
    /// Модель заказа в базе данных
    /// Соответствует официальному описанию JSON-структуры
    /// </summary>
    public class Order
    {
        public int Id { get; set; }
        
        [Required]
        public long UserId { get; set; }
        
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 100 символов")]
        [JsonPropertyName("FirstName")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Фамилия должна содержать от 2 до 100 символов")]
        [JsonPropertyName("LastName")]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [RegularExpression(@"^\d{10}$|^\d{12}$", ErrorMessage = "ИНН должен содержать 10 или 12 цифр")]
        [JsonPropertyName("INN")]
        public string INN { get; set; } = string.Empty;
        
        [Required]
        [Phone(ErrorMessage = "Неверный формат телефона")]
        [RegularExpression(@"^(\+7|8)?[\s\-]?\(?[489][0-9]{2}\)?[\s\-]?[0-9]{3}[\s\-]?[0-9]{2}[\s\-]?[0-9]{2}$", 
            ErrorMessage = "Телефон должен быть в формате +7 (XXX) XXX-XX-XX или 8 (XXX) XXX-XX-XX")]
        [JsonPropertyName("Phone")]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        [JsonPropertyName("Email")]
        public string Email { get; set; } = string.Empty;
        
        [JsonPropertyName("OrderedItems")]
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        
        [JsonPropertyName("TotalPrice")]
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
