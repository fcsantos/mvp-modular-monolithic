using FluentAssertions;
using MyMonolithicApp.Products.Application.Commands;
using MyMonolithicApp.Products.Application.Validators;

namespace MyMonolithicApp.Tests.Products.Application.Validators;

public class UpdateProductCommandValidatorTests
{
    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000", "Valid Product", 10.0, false)]
    [InlineData("d3b07384-d9a0-4f3b-8f8d-3b3b3b3b3b3b", "", 10.0, false)]
    [InlineData("d3b07384-d9a0-4f3b-8f8d-3b3b3b3b3b3b", "Another Product", -1, false)]
    [InlineData("d3b07384-d9a0-4f3b-8f8d-3b3b3b3b3b3b", "Valid Product", 10.0, true)]
    public void Validate_ShouldValidateCorrectly(Guid id, string name, decimal price, bool expectedIsValid)
    {
        // Arrange
        var validator = new UpdateProductCommandValidator();
        var command = new UpdateProductCommand
        {
            Id = id,
            Name = name,
            Price = price
        };

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.Should().Be(expectedIsValid);
    }
}
