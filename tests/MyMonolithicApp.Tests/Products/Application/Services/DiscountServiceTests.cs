using FluentAssertions;
using MyMonolithicApp.Products.Application.Services;

namespace MyMonolithicApp.Tests.Products.Application.Services
{
    public class DiscountServiceTests
    {
        [Theory]
        [InlineData(100, 10, 90)]
        [InlineData(500, 10, 450)]
        [InlineData(200, 0, 200)]
        [InlineData(200, 100, 0)]
        public void ApplyDiscount_ShouldReturnExpectedValue(decimal price, decimal percentage, decimal expected)
        {
            // Arrange
            var service = new DiscountService();

            // Act
            var finalPrice = service.ApplyDiscount(price, percentage);

            // Assert
            finalPrice.Should().Be(expected);
        }

        [Fact]
        public void ApplyDiscount_ShouldThrow_WhenPercentageIsOutOfRange()
        {
            // Arrange
            var service = new DiscountService();

            // Act
            var act = () => service.ApplyDiscount(100, 150);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(400, 400)] // <= 500, sem desconto
        [InlineData(600, 540)] // > 500, aplica 10%
        public void ApplyConditionalDiscount_ShouldApplyDiscountIfOver500(decimal input, decimal expected)
        {
            // Arrange
            var service = new DiscountService();

            // Act
            var result = service.ApplyConditionalDiscount(input);

            // Assert
            result.Should().Be(expected);
        }
    }
}