using System.Text.Json.Serialization;

namespace TMKMiniApp.Models.JsonModels
{
    /// <summary>
    /// ✅ verified - Модель для элемента типа из types.json
    /// Соответствует официальному описанию JSON-структуры
    /// </summary>
    public class TypeEl
    {
        [JsonPropertyName("IDType")]
        public string IDType { get; set; } = string.Empty;

        [JsonPropertyName("Type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("IDParentType")]
        public string IDParentType { get; set; } = string.Empty;
    }
}
