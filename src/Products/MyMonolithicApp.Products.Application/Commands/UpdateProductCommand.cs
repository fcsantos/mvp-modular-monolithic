using MediatR;
using MyMonolithicApp.Products.Application.DTOs;

namespace MyMonolithicApp.Products.Application.Commands
{
    public class UpdateProductCommand : IRequest<ProductDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        // [Opcional] Se quiser separar
        public Guid? RouteId { get; set; }

        public UpdateProductCommand(Guid id, string name, decimal price, Guid? routeId)
        {
            Id = id;
            Name = name;
            Price = price;
            RouteId = routeId;
        }

        // Construtor vazio opcional para permitir Model Binding
        public UpdateProductCommand() { }
    }
}
