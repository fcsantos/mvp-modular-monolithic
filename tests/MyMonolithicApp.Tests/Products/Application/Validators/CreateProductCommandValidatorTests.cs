using FluentAssertions;
using MyMonolithicApp.Products.Application.Commands;
using MyMonolithicApp.Products.Application.Validators;

namespace MyMonolithicApp.Tests.Products.Application.Validators;

public class CreateProductCommandValidatorTests
{
    [Theory]
    [InlineData("Valid Product", 10.0, true)]
    [InlineData("", 10.0, false)]
    [InlineData("Another Product", -1, false)]
    public void Validate_ShouldValidateCorrectly(string name, decimal price, bool expectedIsValid)
    {
        // Arrange
        var validator = new CreateProductCommandValidator();
        var command = new CreateProductCommand
        {
            Name = name,
            Price = price
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().Be(expectedIsValid);
    }
}