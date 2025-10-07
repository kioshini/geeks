# JSON Models для TMK Mini App

Этот каталог содержит модели C# для десериализации JSON-файлов данных.

## 📁 Структура файлов

### Основные модели элементов:
- `NomenclatureEl.cs` - Элемент номенклатуры
- `PricesEl.cs` - Элемент цен
- `RemnantsEl.cs` - Элемент остатков
- `StockEl.cs` - Элемент склада
- `TypeEl.cs` - Элемент типа

### Корневые модели:
- `NomenclatureRoot.cs` - Корневая модель для nomenclature.json
- `PricesRoot.cs` - Корневая модель для prices.json
- `RemnantsRoot.cs` - Корневая модель для remnants.json
- `StocksRoot.cs` - Корневая модель для stocks.json
- `TypesRoot.cs` - Корневая модель для types.json

## 🔧 Использование

### 1. Десериализация JSON-файлов

```csharp
using TMKMiniApp.Models.JsonModels;
using System.Text.Json;

// Загрузка номенклатуры
var json = await File.ReadAllTextAsync("Data/nomenclature.json");
var nomenclature = JsonSerializer.Deserialize<NomenclatureRoot>(json);

// Доступ к данным
foreach (var item in nomenclature.ArrayOfNomenclatureEl)
{
    Console.WriteLine($"ID: {item.ID}, Name: {item.Name}");
}
```

### 2. Использование JsonDataService

```csharp
// В контроллере или сервисе
public class MyController : ControllerBase
{
    private readonly JsonDataService _jsonDataService;
    
    public MyController(JsonDataService jsonDataService)
    {
        _jsonDataService = jsonDataService;
    }
    
    public async Task<IActionResult> GetData()
    {
        var nomenclature = await _jsonDataService.LoadNomenclatureAsync();
        var prices = await _jsonDataService.LoadPricesAsync();
        // ... остальные данные
    }
}
```

### 3. Проверка корректности десериализации

```csharp
// Проверка всех JSON-файлов
var results = await _jsonDataService.ValidateJsonFilesAsync();
foreach (var result in results)
{
    Console.WriteLine($"{result.Key}: {(result.Value ? "✅" : "❌")}");
}
```

## 🧪 Тестирование

Запустите unit-тесты для проверки корректности десериализации:

```bash
dotnet test --filter "JsonDeserializationTests"
```

## 📋 API Endpoints

После запуска приложения доступны следующие endpoints:

- `GET /api/JsonData/validate` - Проверка всех JSON-файлов
- `GET /api/JsonData/nomenclature` - Загрузка номенклатуры
- `GET /api/JsonData/prices` - Загрузка цен
- `GET /api/JsonData/remnants` - Загрузка остатков
- `GET /api/JsonData/stocks` - Загрузка складов
- `GET /api/JsonData/types` - Загрузка типов

## ✅ Статус моделей

Все модели помечены как `✅ verified` и полностью соответствуют структуре JSON-файлов:

- ✅ `NomenclatureEl` - соответствует nomenclature.json
- ✅ `PricesEl` - соответствует prices.json
- ✅ `RemnantsEl` - соответствует remnants.json
- ✅ `StockEl` - соответствует stocks.json
- ✅ `TypeEl` - соответствует types.json

## 🔍 Особенности

1. **Типы данных**: Все типы строго соответствуют JSON (double, int, string, bool)
2. **JsonPropertyName**: Используются атрибуты для точного соответствия имен полей
3. **Nullable типы**: Строки могут быть null, числовые типы имеют значения по умолчанию
4. **Корневые обертки**: Каждый JSON-файл имеет соответствующую корневую модель с массивом элементов
