using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TMKMiniApp.Models.JsonModels
{
    /// <summary>
    /// ✅ verified - Корневая модель для remnants.json
    /// </summary>
    public class RemnantsRoot
    {
        [JsonPropertyName("ArrayOfRemnantsEl")]
        public List<RemnantsEl> ArrayOfRemnantsEl { get; set; } = new();
    }
}
