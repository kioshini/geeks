using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TMKMiniApp.Models.JsonModels
{
    /// <summary>
    /// ✅ verified - Корневая модель для stocks.json
    /// </summary>
    public class StocksRoot
    {
        [JsonPropertyName("ArrayOfStockEl")]
        public List<StockEl> ArrayOfStockEl { get; set; } = new();
    }
}
