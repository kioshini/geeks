using System.Text.Json;
using TMKMiniApp.Models.JsonModels;

namespace TMKMiniApp.Services
{
    /// <summary>
    /// Сервис для загрузки и десериализации JSON-данных
    /// </summary>
    public class JsonDataService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonDataService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <summary>
        /// Загружает номенклатуру из JSON-файла
        /// </summary>
        public async Task<NomenclatureRoot> LoadNomenclatureAsync()
        {
            var filePath = Path.Combine(_environment.ContentRootPath, "Data", "nomenclature.json");
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<NomenclatureRoot>(json, _jsonOptions) ?? new NomenclatureRoot();
        }

        /// <summary>
        /// Загружает цены из JSON-файла
        /// </summary>
        public async Task<PricesRoot> LoadPricesAsync()
        {
            var filePath = Path.Combine(_environment.ContentRootPath, "Data", "prices.json");
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<PricesRoot>(json, _jsonOptions) ?? new PricesRoot();
        }

        /// <summary>
        /// Загружает остатки из JSON-файла
        /// </summary>
        public async Task<RemnantsRoot> LoadRemnantsAsync()
        {
            var filePath = Path.Combine(_environment.ContentRootPath, "Data", "remnants.json");
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<RemnantsRoot>(json, _jsonOptions) ?? new RemnantsRoot();
        }

        /// <summary>
        /// Загружает склады из JSON-файла
        /// </summary>
        public async Task<StocksRoot> LoadStocksAsync()
        {
            var filePath = Path.Combine(_environment.ContentRootPath, "Data", "stocks.json");
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<StocksRoot>(json, _jsonOptions) ?? new StocksRoot();
        }

        /// <summary>
        /// Загружает типы из JSON-файла
        /// </summary>
        public async Task<TypesRoot> LoadTypesAsync()
        {
            var filePath = Path.Combine(_environment.ContentRootPath, "Data", "types.json");
            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<TypesRoot>(json, _jsonOptions) ?? new TypesRoot();
        }

        /// <summary>
        /// Проверяет корректность десериализации всех JSON-файлов
        /// </summary>
        public async Task<Dictionary<string, bool>> ValidateJsonFilesAsync()
        {
            var results = new Dictionary<string, bool>();

            try
            {
                var nomenclature = await LoadNomenclatureAsync();
                results["nomenclature.json"] = nomenclature.ArrayOfNomenclatureEl?.Count > 0;
            }
            catch (Exception ex)
            {
                results["nomenclature.json"] = false;
                Console.WriteLine($"Ошибка загрузки nomenclature.json: {ex.Message}");
            }

            try
            {
                var prices = await LoadPricesAsync();
                results["prices.json"] = prices.ArrayOfPricesEl?.Count > 0;
            }
            catch (Exception ex)
            {
                results["prices.json"] = false;
                Console.WriteLine($"Ошибка загрузки prices.json: {ex.Message}");
            }

            try
            {
                var remnants = await LoadRemnantsAsync();
                results["remnants.json"] = remnants.ArrayOfRemnantsEl?.Count > 0;
            }
            catch (Exception ex)
            {
                results["remnants.json"] = false;
                Console.WriteLine($"Ошибка загрузки remnants.json: {ex.Message}");
            }

            try
            {
                var stocks = await LoadStocksAsync();
                results["stocks.json"] = stocks.ArrayOfStockEl?.Count > 0;
            }
            catch (Exception ex)
            {
                results["stocks.json"] = false;
                Console.WriteLine($"Ошибка загрузки stocks.json: {ex.Message}");
            }

            try
            {
                var types = await LoadTypesAsync();
                results["types.json"] = types.ArrayOfTypeEl?.Count > 0;
            }
            catch (Exception ex)
            {
                results["types.json"] = false;
                Console.WriteLine($"Ошибка загрузки types.json: {ex.Message}");
            }

            return results;
        }
    }
}
