using TMKMiniApp.Models;
using TMKMiniApp.Models.JsonModels;

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
        /// Calculates discount for a product based on quantity and unit using JSON price data
        /// </summary>
        public DiscountInfo CalculateDiscount(Product product, decimal quantity, string unit = "шт", PricesEl? priceData = null)
        {
            var discountInfo = new DiscountInfo
            {
                OriginalPrice = product.Price,
                Quantity = quantity,
                Unit = unit
            };

            if (priceData == null)
            {
                _logger.LogWarning("No price data provided for discount calculation for product {ProductId}", product.Id);
                discountInfo.FinalPrice = product.Price;
                return discountInfo;
            }

            // Calculate discount based on unit and quantity
            if (unit == "т" || unit == "тонн")
            {
                CalculateTonnageDiscount(discountInfo, priceData, quantity);
            }
            else if (unit == "м" || unit == "метр")
            {
                CalculateMeterDiscount(discountInfo, priceData, quantity);
            }
            else
            {
                // For other units, use base price
                discountInfo.FinalPrice = product.Price;
                discountInfo.AppliedDiscounts.Add("Базовая цена (единица измерения не поддерживает скидки)");
            }

            return discountInfo;
        }

        /// <summary>
        /// Calculates discount for tonnage-based pricing
        /// </summary>
        private void CalculateTonnageDiscount(DiscountInfo discountInfo, PricesEl priceData, decimal quantity)
        {
            var appliedDiscounts = new List<string>();
            decimal finalPrice = (decimal)priceData.PriceT;

            // Check first tier discount (PriceLimitT1 -> PriceT1)
            if (quantity >= (decimal)priceData.PriceLimitT1 && priceData.PriceLimitT1 > 0)
            {
                finalPrice = (decimal)priceData.PriceT1;
                var discountPercent = CalculateDiscountPercent(priceData.PriceT, priceData.PriceT1);
                appliedDiscounts.Add($"Скидка 1-й уровень: {discountPercent:F1}% (от {priceData.PriceLimitT1} т)");
                _logger.LogInformation("Applied tonnage discount tier 1: {Percent}% for quantity {Quantity} tons", 
                    discountPercent, quantity);
            }

            // Check second tier discount (PriceLimitT2 -> PriceT2)
            if (quantity >= (decimal)priceData.PriceLimitT2 && priceData.PriceLimitT2 > 0)
            {
                finalPrice = (decimal)priceData.PriceT2;
                var discountPercent = CalculateDiscountPercent(priceData.PriceT, priceData.PriceT2);
                appliedDiscounts.Add($"Скидка 2-й уровень: {discountPercent:F1}% (от {priceData.PriceLimitT2} т)");
                _logger.LogInformation("Applied tonnage discount tier 2: {Percent}% for quantity {Quantity} tons", 
                    discountPercent, quantity);
            }

            discountInfo.FinalPrice = finalPrice;
            discountInfo.DiscountAmount = (decimal)priceData.PriceT - finalPrice;
            discountInfo.TotalDiscountPercent = CalculateDiscountPercent(priceData.PriceT, (double)finalPrice);
            discountInfo.AppliedDiscounts = appliedDiscounts;

            if (appliedDiscounts.Count == 0)
            {
                discountInfo.AppliedDiscounts.Add("Базовая цена (недостаточно количества для скидки)");
            }
        }

        /// <summary>
        /// Calculates discount for meter-based pricing
        /// </summary>
        private void CalculateMeterDiscount(DiscountInfo discountInfo, PricesEl priceData, decimal quantity)
        {
            var appliedDiscounts = new List<string>();
            decimal finalPrice = (decimal)priceData.PriceM;

            // Check first tier discount (PriceLimitM1 -> PriceM1)
            if (quantity >= (decimal)priceData.PriceLimitM1 && priceData.PriceLimitM1 > 0)
            {
                finalPrice = (decimal)priceData.PriceM1;
                var discountPercent = CalculateDiscountPercent(priceData.PriceM, priceData.PriceM1);
                appliedDiscounts.Add($"Скидка 1-й уровень: {discountPercent:F1}% (от {priceData.PriceLimitM1} м)");
                _logger.LogInformation("Applied meter discount tier 1: {Percent}% for quantity {Quantity} meters", 
                    discountPercent, quantity);
            }

            // Check second tier discount (PriceLimitM2 -> PriceM2)
            if (quantity >= (decimal)priceData.PriceLimitM2 && priceData.PriceLimitM2 > 0)
            {
                finalPrice = (decimal)priceData.PriceM2;
                var discountPercent = CalculateDiscountPercent(priceData.PriceM, priceData.PriceM2);
                appliedDiscounts.Add($"Скидка 2-й уровень: {discountPercent:F1}% (от {priceData.PriceLimitM2} м)");
                _logger.LogInformation("Applied meter discount tier 2: {Percent}% for quantity {Quantity} meters", 
                    discountPercent, quantity);
            }

            discountInfo.FinalPrice = finalPrice;
            discountInfo.DiscountAmount = (decimal)priceData.PriceM - finalPrice;
            discountInfo.TotalDiscountPercent = CalculateDiscountPercent(priceData.PriceM, (double)finalPrice);
            discountInfo.AppliedDiscounts = appliedDiscounts;

            if (appliedDiscounts.Count == 0)
            {
                discountInfo.AppliedDiscounts.Add("Базовая цена (недостаточно количества для скидки)");
            }
        }

        /// <summary>
        /// Calculates discount percentage
        /// </summary>
        private decimal CalculateDiscountPercent(double originalPrice, double discountedPrice)
        {
            if (originalPrice <= 0) return 0;
            return (decimal)((originalPrice - discountedPrice) / originalPrice * 100);
        }

        /// <summary>
        /// Calculates final price after applying discounts
        /// </summary>
        public decimal CalculateFinalPrice(Product product, decimal quantity, string unit = "шт", PricesEl? priceData = null)
        {
            var discountInfo = CalculateDiscount(product, quantity, unit, priceData);
            return discountInfo.FinalPrice;
        }
    }
}
