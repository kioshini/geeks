using System.Text.Json.Serialization;

namespace TMKMiniApp.Models.JsonModels
{
    /// <summary>
    /// ✅ verified - Модель для элемента остатков из remnants.json
    /// Соответствует официальному описанию JSON-структуры
    /// ID - ссылка на Nomenclature.ID
    /// </summary>
    public class RemnantsEl
    {
        [JsonPropertyName("ID")]
        public string ID { get; set; } = string.Empty;

        [JsonPropertyName("IDStock")]
        public string IDStock { get; set; } = string.Empty;

        [JsonPropertyName("InStockT")]
        public double InStockT { get; set; }

        [JsonPropertyName("InStockM")]
        public double InStockM { get; set; }

        [JsonPropertyName("SoonArriveT")]
        public double SoonArriveT { get; set; }

        [JsonPropertyName("SoonArriveM")]
        public double SoonArriveM { get; set; }

        [JsonPropertyName("ReservedT")]
        public double ReservedT { get; set; }

        [JsonPropertyName("ReservedM")]
        public double ReservedM { get; set; }

        [JsonPropertyName("UnderTheOrder")]
        public bool UnderTheOrder { get; set; }

        [JsonPropertyName("AvgTubeLength")]
        public double AvgTubeLength { get; set; }

        [JsonPropertyName("AvgTubeWeight")]
        public double AvgTubeWeight { get; set; }
    }
}
