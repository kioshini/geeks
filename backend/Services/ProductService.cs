using TMKMiniApp.Models;
using TMKMiniApp.Models.DTOs;
using TMKMiniApp.Models.JsonModels;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace TMKMiniApp.Services
{
    public class ProductService : IProductService
    {
        private readonly List<Product> _products;
        private readonly List<ProductType> _types;
        private readonly IDiscountService _discountService;
        private readonly ILogger<ProductService> _logger;
        private readonly JsonDataService _jsonDataService;
        private int _nextProductId = 1;
        private int _nextTypeId = 1;

        public ProductService(IDiscountService discountService, ILogger<ProductService> logger, JsonDataService jsonDataService)
        {
            _products = new List<Product>();
            _types = new List<ProductType>();
            _discountService = discountService;
            _logger = logger;
            _jsonDataService = jsonDataService;
            InitializeData();
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            return await Task.FromResult(_products.Select(p => MapToDto(p)));
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            return await Task.FromResult(product != null ? MapToDto(product) : null);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByTypeAsync(string type)
        {
            var products = _products.Where(p => p.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
            return await Task.FromResult(products.Select(p => MapToDto(p)));
        }

        public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
        {
            var products = _products.Where(p => 
                p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true);
            
            return await Task.FromResult(products.Select(p => MapToDto(p)));
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            var product = new Product
            {
                Id = _nextProductId++,
                Name = createProductDto.Name,
                Code = createProductDto.Code,
                Description = createProductDto.Description,
                Type = createProductDto.Type,
                Material = createProductDto.Material,
                Diameter = createProductDto.Diameter,
                Length = createProductDto.Length,
                Thickness = createProductDto.Thickness,
                Unit = createProductDto.Unit ?? "шт",
                Price = createProductDto.Price,
                StockQuantity = createProductDto.StockQuantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _products.Add(product);
            return await Task.FromResult(MapToDto(product));
        }

        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return null;

            if (updateProductDto.Name != null) product.Name = updateProductDto.Name;
            if (updateProductDto.Code != null) product.Code = updateProductDto.Code;
            if (updateProductDto.Description != null) product.Description = updateProductDto.Description;
            if (updateProductDto.Type != null) product.Type = updateProductDto.Type;
            if (updateProductDto.Material != null) product.Material = updateProductDto.Material;
            if (updateProductDto.Diameter.HasValue) product.Diameter = updateProductDto.Diameter;
            if (updateProductDto.Length.HasValue) product.Length = updateProductDto.Length;
            if (updateProductDto.Thickness.HasValue) product.Thickness = updateProductDto.Thickness;
            if (updateProductDto.Unit != null) product.Unit = updateProductDto.Unit;
            if (updateProductDto.Price.HasValue) product.Price = updateProductDto.Price.Value;
            if (updateProductDto.StockQuantity.HasValue) product.StockQuantity = updateProductDto.StockQuantity.Value;

            product.UpdatedAt = DateTime.UtcNow;

            return await Task.FromResult(MapToDto(product));
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return false;

            _products.Remove(product);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateStockAsync(int productId, int quantity)
        {
            var product = _products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return false;

            product.StockQuantity = quantity;
            product.UpdatedAt = DateTime.UtcNow;

            return await Task.FromResult(true);
        }

        public async Task<IEnumerable<ProductType>> GetProductTypesAsync()
        {
            return await Task.FromResult(_types);
        }

        private ProductDto MapToDto(Product product, decimal quantity = 1, string unit = "шт")
        {
            var discountInfo = _discountService.CalculateDiscount(product, quantity, unit);
            
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Code = product.Code,
                Description = product.Description,
                Type = product.Type,
                Material = product.Material,
                Diameter = product.Diameter,
                Length = product.Length,
                Thickness = product.Thickness,
                Unit = product.Unit,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsAvailable = product.IsAvailable,
                
                // Discount information
                VolumeDiscountThreshold = product.VolumeDiscountThreshold,
                VolumeDiscountPercent = product.VolumeDiscountPercent,
                MaterialDiscountPercent = product.MaterialDiscountPercent,
                FinalPrice = discountInfo.FinalPrice,
                DiscountAmount = discountInfo.DiscountAmount,
                TotalDiscountPercent = discountInfo.TotalDiscountPercent,
                HasVolumeDiscount = discountInfo.HasVolumeDiscount,
                HasMaterialDiscount = discountInfo.HasMaterialDiscount
            };
        }

        private void InitializeData()
        {
            // Попробовать загрузить данные из файлов. Если не получилось — создать демо-данные
            var loaded = TryLoadFromFiles();
            if (loaded)
            {
                return;
            }

            // Фолбэк: демо-данные
            _types.AddRange(new[]
            {
                new ProductType { Id = _nextTypeId++, Name = "Трубы стальные", Category = "Металлопрокат", IsActive = true },
                new ProductType { Id = _nextTypeId++, Name = "Трубы полиэтиленовые", Category = "Пластик", IsActive = true },
                new ProductType { Id = _nextTypeId++, Name = "Трубы медные", Category = "Цветной металл", IsActive = true },
                new ProductType { Id = _nextTypeId++, Name = "Трубы чугунные", Category = "Чугун", IsActive = true }
            });

            _products.AddRange(new[]
            {
                new Product
                {
                    Id = _nextProductId++,
                    Name = "Труба стальная 20x2.5",
                    Code = "ST20x2.5",
                    Description = "Стальная труба диаметром 20мм, толщина стенки 2.5мм",
                    Type = "Трубы стальные",
                    Material = "Сталь",
                    Diameter = 20m,
                    Thickness = 2.5m,
                    Unit = "м",
                    Price = 150.00m,
                    StockQuantity = 100,
                    VolumeDiscountThreshold = 100m,
                    VolumeDiscountPercent = 5m,
                    MaterialDiscountPercent = 2m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = _nextProductId++,
                    Name = "Труба ПНД 32x2.4",
                    Code = "PE32x2.4",
                    Description = "Полиэтиленовая труба низкого давления диаметром 32мм",
                    Type = "Трубы полиэтиленовые",
                    Material = "Полиэтилен",
                    Diameter = 32m,
                    Thickness = 2.4m,
                    Unit = "м",
                    Price = 85.00m,
                    StockQuantity = 200,
                    VolumeDiscountThreshold = 50m,
                    VolumeDiscountPercent = 3m,
                    MaterialDiscountPercent = 1m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = _nextProductId++,
                    Name = "Труба медная 15x1.5",
                    Code = "CU15x1.5",
                    Description = "Медная труба для водопровода диаметром 15мм",
                    Type = "Трубы медные",
                    Material = "Медь",
                    Diameter = 15m,
                    Thickness = 1.5m,
                    Unit = "м",
                    Price = 450.00m,
                    StockQuantity = 50,
                    VolumeDiscountThreshold = 20m,
                    VolumeDiscountPercent = 8m,
                    MaterialDiscountPercent = 5m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            });
        }

        private bool TryLoadFromFiles()
        {
            try
            {
                // Пути к файлам в папке Data
                var root = "/home/kioshi/apps/RealHackaton/backend";
                var nomenclaturePath = Path.Combine(root, "Data", "nomenclature.json");
                var typesPath = Path.Combine(root, "Data", "types.json");
                var pricesPath = Path.Combine(root, "Data", "prices.json");
                var remnantsPath = Path.Combine(root, "Data", "remnants.json");
                var stocksPath = Path.Combine(root, "Data", "stocks.json");

                if (!File.Exists(nomenclaturePath))
                {
                    return false;
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };

                // Типы: если файл отсутствует, создадим из ProductionType
                var nomenclatureDoc = JsonSerializer.Deserialize<NomenclatureRoot>(File.ReadAllText(nomenclaturePath), jsonOptions);
                if (nomenclatureDoc?.ArrayOfNomenclatureEl == null || nomenclatureDoc.ArrayOfNomenclatureEl.Count == 0)
                {
                    return false;
                }

                var idToPrice = new Dictionary<string, decimal>();
                if (File.Exists(pricesPath))
                {
                    // Ожидаемый формат неизвестен; если есть поля ID/Price, заполним
                    try
                    {
                        var pricesJson = JsonDocument.Parse(File.ReadAllText(pricesPath));
                        foreach (var el in pricesJson.RootElement.EnumerateArray())
                        {
                            if (el.TryGetProperty("ID", out var idProp))
                            {
                                var id = idProp.GetString() ?? string.Empty;
                                if (el.TryGetProperty("Price", out var priceProp))
                                {
                                    if (decimal.TryParse(priceProp.ToString(), out var price))
                                    {
                                        idToPrice[id] = price;
                                    }
                                }
                            }
                        }
                    }
                    catch { /* best-effort */ }
                }

                var idToStock = new Dictionary<string, int>();
                // Пытаемся взять остатки из remnants или stocks по полю ID/Quantity
                foreach (var path in new[] { remnantsPath, stocksPath })
                {
                    if (File.Exists(path))
                    {
                        try
                        {
                            var doc = JsonDocument.Parse(File.ReadAllText(path));
                            foreach (var el in doc.RootElement.EnumerateArray())
                            {
                                var id = el.TryGetProperty("ID", out var idProp) ? (idProp.GetString() ?? string.Empty) : string.Empty;
                                if (string.IsNullOrEmpty(id)) continue;
                                int qty = 0;
                                if (el.TryGetProperty("Quantity", out var qProp))
                                {
                                    int.TryParse(qProp.ToString(), out qty);
                                }
                                else if (el.TryGetProperty("Remnant", out var rProp))
                                {
                                    int.TryParse(rProp.ToString(), out qty);
                                }
                                if (qty > 0)
                                {
                                    idToStock[id] = qty;
                                }
                            }
                        }
                        catch { /* best-effort */ }
                    }
                }

                _products.Clear();
                _types.Clear();
                _nextProductId = 1;
                _nextTypeId = 1;

                var typeNameSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var n in nomenclatureDoc.ArrayOfNomenclatureEl)
                {
                    // Вычисление/сбор полей
                    var name = n.Name ?? ($"Труба D {n.Diameter} S {n.PipeWallThickness}");
                    var code = n.ID ?? Guid.NewGuid().ToString("N");
                    var typeName = string.IsNullOrWhiteSpace(n.ProductionType) ? "Прочее" : n.ProductionType.Trim();
                    var material = n.SteelGrade ?? "Сталь";
                    decimal? diameter = n.Diameter.HasValue ? (decimal?)Convert.ToDecimal(n.Diameter.Value) : null;
                    decimal? thickness = n.PipeWallThickness.HasValue ? (decimal?)Convert.ToDecimal(n.PipeWallThickness.Value) : null;

                    // Цена: из файла цен, иначе грубая оценка по коэффициенту (если есть)
                    decimal price = 0m;
                    if (idToPrice.TryGetValue(code, out var p))
                    {
                        price = p;
                    }
                    else if (n.Koef.HasValue)
                    {
                        // грубая цена как коэффициент * 100000 (для демонстрации)
                        price = Math.Round((decimal)n.Koef.Value * 100000m, 2);
                    }

                    var stock = 0;
                    if (idToStock.TryGetValue(code, out var s))
                    {
                        stock = s;
                    }
                    // Если нет файла остатков или запись не найдена — считаем в наличии по умолчанию
                    if (stock <= 0)
                    {
                        stock = 100;
                    }

                    // Добавляем тип, если ещё не добавлен
                    if (typeNameSet.Add(typeName))
                    {
                        _types.Add(new ProductType
                        {
                            Id = _nextTypeId++,
                            Name = typeName,
                            Category = null,
                            Description = null,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        });
                    }

                    _products.Add(new Product
                    {
                        Id = _nextProductId++,
                        Name = name,
                        Code = code,
                        Description = n.Gost,
                        Type = typeName,
                        Material = material,
                        Diameter = diameter,
                        Thickness = thickness,
                        Unit = "м",
                        Price = price,
                        StockQuantity = stock,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                return _products.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        private sealed class NomenclatureRoot
        {
            [JsonPropertyName("ArrayOfNomenclatureEl")]
            public List<NomenclatureEl> ArrayOfNomenclatureEl { get; set; } = new();
        }

        private sealed class NomenclatureEl
        {
            public string? ID { get; set; }
            public string? Name { get; set; }
            public string? ProductionType { get; set; }
            public string? Gost { get; set; }
            public string? SteelGrade { get; set; }
            public double? Diameter { get; set; }
            public double? PipeWallThickness { get; set; }
            public double? Koef { get; set; }
        }

        #region Delta Update Methods

        /// <summary>
        /// Обновляет цену товара с учетом дельты
        /// </summary>
        public async Task<bool> UpdatePriceDeltaAsync(string productId, string stockId, PricesEl priceDelta)
        {
            try
            {
                var product = await GetProductByNomenclatureIdAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Товар с ID номенклатуры {ProductId} не найден", productId);
                    return false;
                }

                // Применяем дельты к ценам
                var newPriceT = Math.Max(0, (decimal)product.Price + (decimal)priceDelta.PriceT);
                var newPriceM = Math.Max(0, (decimal)product.Price + (decimal)priceDelta.PriceM);

                // Обновляем цену товара (используем среднее значение или основную цену)
                product.Price = (newPriceT + newPriceM) / 2;
                product.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Обновлена цена товара {ProductId}: {OldPrice} -> {NewPrice} (дельта T: {DeltaT}, M: {DeltaM})", 
                    productId, product.Price, product.Price, priceDelta.PriceT, priceDelta.PriceM);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении дельты цены для товара {ProductId}", productId);
                return false;
            }
        }

        /// <summary>
        /// Обновляет остаток товара с учетом дельты
        /// </summary>
        public async Task<bool> UpdateStockDeltaAsync(string productId, string stockId, RemnantsEl remnantDelta)
        {
            try
            {
                var product = await GetProductByNomenclatureIdAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Товар с ID номенклатуры {ProductId} не найден", productId);
                    return false;
                }

                // Применяем дельты к остаткам
                var newStockT = Math.Max(0, product.StockQuantity + (int)remnantDelta.InStockT);
                var newStockM = Math.Max(0, product.StockQuantity + (int)remnantDelta.InStockM);

                // Обновляем остаток товара (используем среднее значение)
                product.StockQuantity = (newStockT + newStockM) / 2;
                product.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Обновлен остаток товара {ProductId}: {OldStock} -> {NewStock} (дельта T: {DeltaT}, M: {DeltaM})", 
                    productId, product.StockQuantity, product.StockQuantity, remnantDelta.InStockT, remnantDelta.InStockM);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении дельты остатка для товара {ProductId}", productId);
                return false;
            }
        }

        /// <summary>
        /// Получает товар по ID номенклатуры
        /// </summary>
        public async Task<Product?> GetProductByNomenclatureIdAsync(string nomenclatureId)
        {
            try
            {
                // В реальной реализации здесь должен быть поиск по ID номенклатуры
                // Пока что возвращаем первый товар для демонстрации
                var products = await GetAllProductsAsync();
                // Временная реализация - возвращаем null
                // В реальной системе здесь должен быть поиск по ID номенклатуры
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при поиске товара по ID номенклатуры {NomenclatureId}", nomenclatureId);
                return null;
            }
        }

        /// <summary>
        /// Обновляет цену товара напрямую
        /// </summary>
        public async Task<bool> UpdateProductPriceAsync(string nomenclatureId, string stockId, decimal newPriceT, decimal newPriceM)
        {
            try
            {
                var product = await GetProductByNomenclatureIdAsync(nomenclatureId);
                if (product == null)
                {
                    return false;
                }

                product.Price = (newPriceT + newPriceM) / 2;
                product.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Обновлена цена товара {NomenclatureId}: {NewPrice}", nomenclatureId, product.Price);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении цены товара {NomenclatureId}", nomenclatureId);
                return false;
            }
        }

        /// <summary>
        /// Обновляет остаток товара напрямую
        /// </summary>
        public async Task<bool> UpdateProductStockAsync(string nomenclatureId, string stockId, decimal newStockT, decimal newStockM)
        {
            try
            {
                var product = await GetProductByNomenclatureIdAsync(nomenclatureId);
                if (product == null)
                {
                    return false;
                }

                product.StockQuantity = (int)((newStockT + newStockM) / 2);
                product.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Обновлен остаток товара {NomenclatureId}: {NewStock}", nomenclatureId, product.StockQuantity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении остатка товара {NomenclatureId}", nomenclatureId);
                return false;
            }
        }

        /// <summary>
        /// Получить данные о ценах для товара
        /// </summary>
        public async Task<PricesEl?> GetPriceDataAsync(string productId)
        {
            try
            {
                var pricesRoot = await _jsonDataService.LoadPricesAsync();
                var prices = pricesRoot.ArrayOfPricesEl ?? new List<PricesEl>();
                return prices.FirstOrDefault(p => p.ID == productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении данных о ценах для товара {ProductId}", productId);
                return null;
            }
        }

        /// <summary>
        /// Получить данные об остатках для товара
        /// </summary>
        public async Task<RemnantsEl?> GetRemnantDataAsync(string productId)
        {
            try
            {
                var remnantsRoot = await _jsonDataService.LoadRemnantsAsync();
                var remnants = remnantsRoot.ArrayOfRemnantsEl ?? new List<RemnantsEl>();
                return remnants.FirstOrDefault(r => r.ID == productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении данных об остатках для товара {ProductId}", productId);
                return null;
            }
        }

        #endregion
    }
}