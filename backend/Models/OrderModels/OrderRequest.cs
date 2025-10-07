using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TMKMiniApp.Models.OrderModels
{
    /// <summary>
    /// Модель запроса на создание заказа с валидацией
    /// Соответствует официальному описанию JSON-структуры
    /// </summary>
    public class OrderRequest
    {
        [Required(ErrorMessage = "Имя обязательно для заполнения")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 100 символов")]
        [JsonPropertyName("FirstName")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Фамилия должна содержать от 2 до 100 символов")]
        [JsonPropertyName("LastName")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "ИНН обязателен для заполнения")]
        [JsonPropertyName("INN")]
        public string INN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Телефон обязателен для заполнения")]
        [Phone(ErrorMessage = "Неверный формат телефона")]
        [RegularExpression(@"^(\+7|8)?[\s\-]?\(?[489][0-9]{2}\)?[\s\-]?[0-9]{3}[\s\-]?[0-9]{2}[\s\-]?[0-9]{2}$", 
            ErrorMessage = "Телефон должен быть в формате +7 (XXX) XXX-XX-XX или 8 (XXX) XXX-XX-XX")]
        [JsonPropertyName("Phone")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен для заполнения")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        [JsonPropertyName("Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Список товаров обязателен")]
        [MinLength(1, ErrorMessage = "Заказ должен содержать хотя бы один товар")]
        [JsonPropertyName("OrderedItems")]
        public List<OrderedItem> OrderedItems { get; set; } = new();

        [JsonPropertyName("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }

    /// <summary>
    /// Модель товара в заказе
    /// </summary>
    public class OrderedItem
    {
        [Required(ErrorMessage = "ID товара обязателен")]
        [JsonPropertyName("ProductId")]
        public string ProductId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Количество обязательно")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть больше 0")]
        [JsonPropertyName("Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Единица измерения обязательна")]
        [JsonPropertyName("Unit")]
        public string Unit { get; set; } = string.Empty; // "т" или "м"

        [JsonPropertyName("UnitPrice")]
        public decimal UnitPrice { get; set; }

        [JsonPropertyName("TotalPrice")]
        public decimal TotalPrice { get; set; }
    }
}
