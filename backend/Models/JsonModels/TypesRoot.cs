using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TMKMiniApp.Models.JsonModels
{
    /// <summary>
    /// ✅ verified - Корневая модель для types.json
    /// </summary>
    public class TypesRoot
    {
        [JsonPropertyName("ArrayOfTypeEl")]
        public List<TypeEl> ArrayOfTypeEl { get; set; } = new();
    }
}
