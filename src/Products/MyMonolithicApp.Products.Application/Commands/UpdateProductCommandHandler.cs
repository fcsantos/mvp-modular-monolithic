using AutoMapper;
using MediatR;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Exceptions;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Products.Application.DTOs;
using MyMonolithicApp.Products.Application.Services;

namespace MyMonolithicApp.Products.Application.Commands
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IMapper _mapper;
        private readonly IDiscountService _discountService;

        public UpdateProductCommandHandler(IRepository<Product> productRepository, IMapper mapper, IDiscountService discountService)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _discountService = discountService;
        }

        public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            // [Exemplo] Validação adicional: ID do command não corresponde ao que esperamos
            // (caso você queira mover esse tipo de validação do Controller para o Handler)
            // Se preferir manter no Controller, basta remover este trecho
            // e ficar apenas com as validações de negócio aqui.
            if (request.RouteId.HasValue && request.RouteId.Value != request.Id)
            {
                // Lança exceção -> Middleware converte em 400
                throw new BadRequestException($"Route ID {request.RouteId.Value} and body ID {request.Id} do not match.");
            }

            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product is null)
            {
                // Ao invés de retornar null, lançamos a NotFoundException
                throw new NotFoundException($"Product with ID {request.Id} was not found.");
            }

            var finalPrice = _discountService.ApplyConditionalDiscount(request.Price);

            // Atualizar as propriedades
            product.Name = request.Name;
            product.Price = finalPrice;

            await _productRepository.UpdateAsync(product);

            return _mapper.Map<ProductDto>(product);
        }
    }
}
