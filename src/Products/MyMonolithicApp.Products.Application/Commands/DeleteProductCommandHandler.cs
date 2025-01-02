using MediatR;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Exceptions;
using MyMonolithicApp.Core.Interfaces;

namespace MyMonolithicApp.Products.Application.Commands
{
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IRepository<Product> _productRepository;

        public DeleteProductCommandHandler(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            // Verifica se o produto existe
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                // Lançamos NotFound => Middleware converte em 404
                throw new NotFoundException($"Product with ID {request.Id} was not found.");
            }

            // Se necessário, verifique outras regras (ex.: produto não pode ser deletado se X).
            // Se a regra falhar, podemos lançar BadRequestException ou outra exceção.

            //Se houver outras regras (ex.: “produto não pode ser deletado se estiver em um pedido ativo”),
            //podemos lançar BadRequestException ou outra exceção.

            await _productRepository.DeleteAsync(request.Id);

            // Retorna "Unit.Value" indicando sucesso sem payload
            return Unit.Value;
        }
    }
}
