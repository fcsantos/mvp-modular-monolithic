using AutoMapper;
using MediatR;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Products.Application.DTOs;
using MyMonolithicApp.Products.Application.Services;

namespace MyMonolithicApp.Products.Application.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly IDiscountService _discountService;

        public CreateProductCommandHandler(IRepository<Product> productRepository, IMapper mapper, IDiscountService discountService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _discountService = discountService;
        }

        public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Se quisermos aplicar desconto acima de 500
            var finalPrice = _discountService.ApplyConditionalDiscount(request.Price);

            var product = new Product
            {
                Name = request.Name,
                Price = finalPrice
            };

            await _productRepository.AddAsync(product);
            return _mapper.Map<ProductDto>(product);
        }
    }
}
