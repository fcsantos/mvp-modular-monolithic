using AutoMapper;
using MediatR;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Exceptions;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Products.Application.DTOs;

namespace MyMonolithicApp.Products.Application.Commands
{
    public class UpdateProductStockCommandHandler : IRequestHandler<UpdateProductStockCommand, ProductDto>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public UpdateProductStockCommandHandler(IRepository<Product> productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(UpdateProductStockCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                throw new NotFoundException($"Product with ID {request.ProductId} was not found.");
            }

            // Use the domain method for stock updates with validation
            product.UpdateStock(request.NewStockQuantity);

            await _productRepository.UpdateAsync(product);
            return _mapper.Map<ProductDto>(product);
        }
    }
}