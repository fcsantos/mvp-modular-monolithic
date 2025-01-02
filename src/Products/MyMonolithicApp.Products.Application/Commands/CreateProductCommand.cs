using MediatR;
using MyMonolithicApp.Products.Application.DTOs;

namespace MyMonolithicApp.Products.Application.Commands
{
    public class CreateProductCommand : IRequest<ProductDto>
    {
        public required string Name { get; set; }
        public decimal Price { get; set; }
    }
}