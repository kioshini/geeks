using System.Text.Json.Serialization;

namespace TMKMiniApp.Models.JsonModels
{
    /// <summary>
    /// ✅ verified - Модель для элемента цен из prices.json
    /// Соответствует официальному описанию JSON-структуры
    /// ID - ссылка на Nomenclature.ID
    /// </summary>
    public class PricesEl
    {
        [JsonPropertyName("ID")]
        public string ID { get; set; } = string.Empty;

        [JsonPropertyName("IDStock")]
        public string IDStock { get; set; } = string.Empty;

        [JsonPropertyName("PriceT")]
        public double PriceT { get; set; }

        [JsonPropertyName("PriceLimitT1")]
        public double PriceLimitT1 { get; set; }

        [JsonPropertyName("PriceT1")]
        public double PriceT1 { get; set; }

        [JsonPropertyName("PriceLimitT2")]
        public double PriceLimitT2 { get; set; }

        [JsonPropertyName("PriceT2")]
        public double PriceT2 { get; set; }

        [JsonPropertyName("PriceM")]
        public double PriceM { get; set; }

        [JsonPropertyName("PriceLimitM1")]
        public double PriceLimitM1 { get; set; }

        [JsonPropertyName("PriceM1")]
        public double PriceM1 { get; set; }

        [JsonPropertyName("PriceLimitM2")]
        public double PriceLimitM2 { get; set; }

        [JsonPropertyName("PriceM2")]
        public double PriceM2 { get; set; }

        [JsonPropertyName("NDS")]
        public int NDS { get; set; }
    }
}
