using TMKMiniApp.Models;

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
        /// <returns>Discount information including percentage and amount</returns>
        DiscountInfo CalculateDiscount(Product product, decimal quantity, string unit = "шт");
        
        /// <summary>
        /// Calculates final price after applying discounts
        /// </summary>
        /// <param name="product">Product to calculate price for</param>
        /// <param name="quantity">Quantity of the product</param>
        /// <param name="unit">Unit of measurement</param>
        /// <returns>Final price after discounts</returns>
        decimal CalculateFinalPrice(Product product, decimal quantity, string unit = "шт");
    }

    /// <summary>
    /// Discount information result
    /// </summary>
    public class DiscountInfo
    {
        public decimal VolumeDiscountPercent { get; set; }
        public decimal MaterialDiscountPercent { get; set; }
        public decimal TotalDiscountPercent { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public bool HasVolumeDiscount { get; set; }
        public bool HasMaterialDiscount { get; set; }
    }
}
