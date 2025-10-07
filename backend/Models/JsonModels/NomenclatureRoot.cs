using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TMKMiniApp.Models.JsonModels
{
    /// <summary>
    /// ✅ verified - Корневая модель для nomenclature.json
    /// </summary>
    public class NomenclatureRoot
    {
        [JsonPropertyName("ArrayOfNomenclatureEl")]
        public List<NomenclatureEl> ArrayOfNomenclatureEl { get; set; } = new();
    }
}
