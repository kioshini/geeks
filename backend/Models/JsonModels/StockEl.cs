using System.Text.Json.Serialization;

namespace TMKMiniApp.Models.JsonModels
{
    /// <summary>
    /// ✅ verified - Модель для элемента склада из stocks.json
    /// Соответствует официальному описанию JSON-структуры
    /// </summary>
    public class StockEl
    {
        [JsonPropertyName("IDStock")]
        public string IDStock { get; set; } = string.Empty;

        [JsonPropertyName("Stock")]
        public string Stock { get; set; } = string.Empty;

        [JsonPropertyName("StockName")]
        public string StockName { get; set; } = string.Empty;

        [JsonPropertyName("Address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("Schedule")]
        public string Schedule { get; set; } = string.Empty;

        [JsonPropertyName("IDDivision")]
        public string IDDivision { get; set; } = string.Empty;

        [JsonPropertyName("CashPayment")]
        public bool CashPayment { get; set; }

        [JsonPropertyName("CardPayment")]
        public bool CardPayment { get; set; }

        [JsonPropertyName("FIASId")]
        public string FIASId { get; set; } = string.Empty;

        [JsonPropertyName("OwnerInn")]
        public string OwnerInn { get; set; } = string.Empty;

        [JsonPropertyName("OwnerKpp")]
        public string OwnerKpp { get; set; } = string.Empty;

        [JsonPropertyName("OwnerFullName")]
        public string OwnerFullName { get; set; } = string.Empty;

        [JsonPropertyName("OwnerShortName")]
        public string OwnerShortName { get; set; } = string.Empty;

        [JsonPropertyName("RailwayStation")]
        public string RailwayStation { get; set; } = string.Empty;

        [JsonPropertyName("ConsigneeCode")]
        public string ConsigneeCode { get; set; } = string.Empty;
    }
}
