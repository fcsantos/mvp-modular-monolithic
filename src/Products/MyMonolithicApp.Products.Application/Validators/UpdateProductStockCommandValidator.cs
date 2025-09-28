using FluentValidation;
using MyMonolithicApp.Products.Application.Commands;

namespace MyMonolithicApp.Products.Application.Validators
{
    public class UpdateProductStockCommandValidator : AbstractValidator<UpdateProductStockCommand>
    {
        public UpdateProductStockCommandValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("Product ID is required.");

            RuleFor(x => x.NewStockQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Stock quantity cannot be negative.");
        }
    }
}