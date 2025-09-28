using MediatR;
using MyMonolithicApp.Products.Application.DTOs;

namespace MyMonolithicApp.Products.Application.Commands
{
    public class UpdateProductCommand : IRequest<ProductDto>
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public int StockQuantity { get; set; }
        public bool IsActive { get; set; }

        // [Opcional] Se quiser separar
        public Guid? RouteId { get; set; }

        public UpdateProductCommand(Guid id, string name, string? description, decimal price, string? category, int stockQuantity, bool isActive, Guid? routeId)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Category = category;
            StockQuantity = stockQuantity;
            IsActive = isActive;
            RouteId = routeId;
        }

        // Construtor vazio opcional para permitir Model Binding
        public UpdateProductCommand() { }
    }
}
