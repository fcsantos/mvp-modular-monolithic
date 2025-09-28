using FluentValidation;
using MyMonolithicApp.Products.Application.Commands;

namespace MyMonolithicApp.Products.Application.Validators
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(cmd => cmd.Id)
                .NotEmpty().WithMessage("Id is required.");

            RuleFor(cmd => cmd.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

            RuleFor(cmd => cmd.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.")
                .When(cmd => !string.IsNullOrEmpty(cmd.Description));

            RuleFor(cmd => cmd.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be 0 or greater.");

            RuleFor(cmd => cmd.Category)
                .MaximumLength(100).WithMessage("Category cannot exceed 100 characters.")
                .When(cmd => !string.IsNullOrEmpty(cmd.Category));

            RuleFor(cmd => cmd.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
        }
    }
}
