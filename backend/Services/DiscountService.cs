using TMKMiniApp.Models;

namespace TMKMiniApp.Services
{
    /// <summary>
    /// Implementation of discount calculation service
    /// </summary>
    public class DiscountService : IDiscountService
    {
        private readonly ILogger<DiscountService> _logger;

        public DiscountService(ILogger<DiscountService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Calculates discount for a product based on quantity and material
        /// </summary>
        public DiscountInfo CalculateDiscount(Product product, decimal quantity, string unit = "шт")
        {
            var discountInfo = new DiscountInfo
            {
                OriginalPrice = product.Price
            };

            // Calculate volume discount
            if (product.VolumeDiscountThreshold.HasValue && 
                product.VolumeDiscountPercent.HasValue && 
                quantity >= product.VolumeDiscountThreshold.Value)
            {
                discountInfo.VolumeDiscountPercent = product.VolumeDiscountPercent.Value;
                discountInfo.HasVolumeDiscount = true;
                _logger.LogInformation("Volume discount applied: {Percent}% for quantity {Quantity}", 
                    product.VolumeDiscountPercent.Value, quantity);
            }

            // Calculate material discount
            if (product.MaterialDiscountPercent.HasValue)
            {
                discountInfo.MaterialDiscountPercent = product.MaterialDiscountPercent.Value;
                discountInfo.HasMaterialDiscount = true;
                _logger.LogInformation("Material discount applied: {Percent}% for material {Material}", 
                    product.MaterialDiscountPercent.Value, product.Material);
            }

            // Calculate total discount (additive)
            discountInfo.TotalDiscountPercent = discountInfo.VolumeDiscountPercent + discountInfo.MaterialDiscountPercent;
            
            // Calculate discount amount and final price
            discountInfo.DiscountAmount = discountInfo.OriginalPrice * (discountInfo.TotalDiscountPercent / 100);
            discountInfo.FinalPrice = discountInfo.OriginalPrice - discountInfo.DiscountAmount;

            return discountInfo;
        }

        /// <summary>
        /// Calculates final price after applying discounts
        /// </summary>
        public decimal CalculateFinalPrice(Product product, decimal quantity, string unit = "шт")
        {
            var discountInfo = CalculateDiscount(product, quantity, unit);
            return discountInfo.FinalPrice;
        }
    }
}
