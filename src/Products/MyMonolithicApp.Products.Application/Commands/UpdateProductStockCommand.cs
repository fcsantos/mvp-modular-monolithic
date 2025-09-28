using MediatR;
using MyMonolithicApp.Products.Application.DTOs;

namespace MyMonolithicApp.Products.Application.Commands
{
    public class UpdateProductStockCommand : IRequest<ProductDto>
    {
        public Guid ProductId { get; set; }
        public int NewStockQuantity { get; set; }

        public UpdateProductStockCommand(Guid productId, int newStockQuantity)
        {
            ProductId = productId;
            NewStockQuantity = newStockQuantity;
        }

        public UpdateProductStockCommand() { }
    }
}