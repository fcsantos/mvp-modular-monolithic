using FluentValidation;
using MediatR;

namespace MyMonolithicApp.Products.Application.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // Se não houver validators para este Request, segue adiante
            if (!_validators.Any())
                return await next();

            // Executa todas as validações configuradas para TRequest
            var context = new ValidationContext<TRequest>(request);
            var validationResults = _validators
                .Select(v => v.Validate(context))
                .ToList();

            // Coleta todas as falhas
            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (failures.Any())
            {
                // Pode lançar uma ValidationException do FluentValidation
                throw new ValidationException(failures);
            }

            // Se não houver falhas, segue o fluxo
            return await next();
        }
    }
}
