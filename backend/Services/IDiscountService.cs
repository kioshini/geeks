using TMKMiniApp.Models;
using TMKMiniApp.Models.JsonModels;

namespace TMKMiniApp.Services
{
    /// <summary>
    /// Service for calculating discounts based on volume and material
    /// </summary>
    public interface IDiscountService
    {
        /// <summary>
        /// Calculates discount for a product based on quantity and material
        /// </summary>
        /// <param name="product">Product to calculate discount for</param>
        /// <param name="quantity">Quantity of the product</param>
        /// <param name="unit">Unit of measurement (meters, tons, etc.)</param>
        /// <param name="priceData">Price data from JSON for discount calculation</param>
        /// <returns>Discount information including percentage and amount</returns>
        DiscountInfo CalculateDiscount(Product product, decimal quantity, string unit = "шт", PricesEl? priceData = null);
        
        /// <summary>
        /// Calculates final price after applying discounts
        /// </summary>
        /// <param name="product">Product to calculate price for</param>
        /// <param name="quantity">Quantity of the product</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="priceData">Price data from JSON for discount calculation</param>
        /// <returns>Final price after discounts</returns>
        decimal CalculateFinalPrice(Product product, decimal quantity, string unit = "шт", PricesEl? priceData = null);
    }
}
