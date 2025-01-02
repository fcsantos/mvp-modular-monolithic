namespace MyMonolithicApp.Products.Application.Services
{
    public interface IDiscountService
    {
        /// <summary>
        /// Aplica um desconto percentual de 0 a 100% sobre um preço.
        /// </summary>
        /// <param name="price">Preço original.</param>
        /// <param name="discountPercentage">Percentual de desconto (0-100).</param>
        /// <returns>Preço final.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Se o percentual for < 0 ou > 100.</exception>
        decimal ApplyDiscount(decimal price, decimal discountPercentage);

        /// <summary>
        /// Exemplo: se o preço for maior que 500, aplica 10% de desconto.
        /// </summary>
        decimal ApplyConditionalDiscount(decimal price);
    }
}
