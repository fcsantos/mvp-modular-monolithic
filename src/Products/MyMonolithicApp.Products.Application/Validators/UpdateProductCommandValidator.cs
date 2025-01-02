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

            RuleFor(cmd => cmd.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be 0 or greater.");
        }
    }
}
