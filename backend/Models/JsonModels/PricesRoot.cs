using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TMKMiniApp.Models.JsonModels
{
    /// <summary>
    /// ✅ verified - Корневая модель для prices.json
    /// </summary>
    public class PricesRoot
    {
        [JsonPropertyName("ArrayOfPricesEl")]
        public List<PricesEl> ArrayOfPricesEl { get; set; } = new();
    }
}
