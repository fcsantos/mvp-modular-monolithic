using MediatR;

namespace MyMonolithicApp.Products.Application.Commands
{
    public class DeleteProductCommand : IRequest<Unit>
    {
        public Guid Id { get; }

        public DeleteProductCommand(Guid id)
        {
            Id = id;
        }
    }
}