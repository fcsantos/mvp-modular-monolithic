using MediatR;
using MyMonolithicApp.Products.Application.DTOs;

namespace MyMonolithicApp.Products.Application.Commands
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public int StockQuantity { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}