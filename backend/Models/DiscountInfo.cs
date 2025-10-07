using System.Text.Json.Serialization;

namespace TMKMiniApp.Models
{
    /// <summary>
    /// Модель для информации о скидках
    /// </summary>
    public class DiscountInfo
    {
        [JsonPropertyName("originalPrice")]
        public decimal OriginalPrice { get; set; }

        [JsonPropertyName("volumeDiscountPercent")]
        public decimal VolumeDiscountPercent { get; set; }

        [JsonPropertyName("materialDiscountPercent")]
        public decimal MaterialDiscountPercent { get; set; }

        [JsonPropertyName("totalDiscountPercent")]
        public decimal TotalDiscountPercent { get; set; }

        [JsonPropertyName("discountAmount")]
        public decimal DiscountAmount { get; set; }

        [JsonPropertyName("finalPrice")]
        public decimal FinalPrice { get; set; }

        [JsonPropertyName("hasVolumeDiscount")]
        public bool HasVolumeDiscount { get; set; }

        [JsonPropertyName("hasMaterialDiscount")]
        public bool HasMaterialDiscount { get; set; }

        [JsonPropertyName("appliedDiscounts")]
        public List<string> AppliedDiscounts { get; set; } = new List<string>();

        [JsonPropertyName("unit")]
        public string Unit { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public decimal Quantity { get; set; }
    }
}
