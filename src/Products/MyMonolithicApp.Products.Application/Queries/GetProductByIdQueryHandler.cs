using AutoMapper;
using MediatR;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Exceptions;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Products.Application.DTOs;

namespace MyMonolithicApp.Products.Application.Queries
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public GetProductByIdQueryHandler(IRepository<Product> productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product is null)
            {
                // Ao invés de retornar null, lançamos exceção NotFound
                throw new NotFoundException($"Product with ID {request.Id} was not found.");
            }
            return _mapper.Map<ProductDto>(product);
        }
    }
}
