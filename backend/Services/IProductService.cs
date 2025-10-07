using TMKMiniApp.Models;
using TMKMiniApp.Models.DTOs;
using TMKMiniApp.Models.JsonModels;

namespace TMKMiniApp.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetProductsByTypeAsync(string type);
        Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> UpdateStockAsync(int productId, int quantity);
        Task<IEnumerable<ProductType>> GetProductTypesAsync();
        
        // Методы для работы с дельтами
        Task<bool> UpdatePriceDeltaAsync(string productId, string stockId, PricesEl priceDelta);
        Task<bool> UpdateStockDeltaAsync(string productId, string stockId, RemnantsEl remnantDelta);
        Task<Product?> GetProductByNomenclatureIdAsync(string nomenclatureId);
        Task<bool> UpdateProductPriceAsync(string nomenclatureId, string stockId, decimal newPriceT, decimal newPriceM);
        Task<bool> UpdateProductStockAsync(string nomenclatureId, string stockId, decimal newStockT, decimal newStockM);
        Task<PricesEl?> GetPriceDataAsync(string productId);
        Task<RemnantsEl?> GetRemnantDataAsync(string productId);
    }
}
