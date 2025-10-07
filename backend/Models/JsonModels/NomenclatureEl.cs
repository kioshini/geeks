using System.Text.Json.Serialization;

namespace TMKMiniApp.Models.JsonModels
{
    /// <summary>
    /// ✅ verified - Модель для элемента номенклатуры из nomenclature.json
    /// Соответствует официальному описанию JSON-структуры
    /// </summary>
    public class NomenclatureEl
    {
        [JsonPropertyName("ID")]
        public string ID { get; set; } = string.Empty;

        [JsonPropertyName("IDCat")]
        public string IDCat { get; set; } = string.Empty;

        [JsonPropertyName("IDType")]
        public string IDType { get; set; } = string.Empty;

        [JsonPropertyName("IDTypeNew")]
        public string IDTypeNew { get; set; } = string.Empty;

        [JsonPropertyName("ProductionType")]
        public string ProductionType { get; set; } = string.Empty;

        [JsonPropertyName("IDFunctionType")]
        public string IDFunctionType { get; set; } = string.Empty;

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("Gost")]
        public string Gost { get; set; } = string.Empty;

        [JsonPropertyName("FormOfLength")]
        public string FormOfLength { get; set; } = string.Empty;

        [JsonPropertyName("Manufacturer")]
        public string Manufacturer { get; set; } = string.Empty;

        [JsonPropertyName("SteelGrade")]
        public string SteelGrade { get; set; } = string.Empty;

        [JsonPropertyName("Diameter")]
        public double Diameter { get; set; }

        [JsonPropertyName("ProfileSize2")]
        public double ProfileSize2 { get; set; }

        [JsonPropertyName("PipeWallThickness")]
        public double PipeWallThickness { get; set; }

        [JsonPropertyName("Status")]
        public int Status { get; set; }

        [JsonPropertyName("Koef")]
        public double Koef { get; set; }
    }
}
