namespace MyMonolithicApp.Products.Application.Services
{
    public class DiscountService : IDiscountService
    {
        public decimal ApplyDiscount(decimal price, decimal discountPercentage)
        {
            if (discountPercentage < 0 || discountPercentage > 100)
                throw new ArgumentOutOfRangeException(nameof(discountPercentage), "Discount must be between 0% and 100%.");

            var discountAmount = price * (discountPercentage / 100);
            return price - discountAmount;
        }

        public decimal ApplyConditionalDiscount(decimal price)
        {
            const decimal threshold = 500m;
            const decimal discountPercent = 10m;

            if (price > threshold)
                return ApplyDiscount(price, discountPercent);

            return price; // Sem desconto
        }
    }
}