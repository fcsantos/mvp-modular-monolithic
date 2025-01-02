using Moq;
using FluentAssertions;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Products.Application.Commands;
using MyMonolithicApp.Products.Application.DTOs;
using AutoMapper;
using MyMonolithicApp.Products.Application.Services;

namespace MyMonolithicApp.Tests.Products.Application.Handlers;

public class CreateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenDataIsValid()
    {
        // Arrange
        var mockRepository = new Mock<IRepository<Product>>();
        var mockMapper = new Mock<IMapper>();
        var mockDiscountService = new Mock<DiscountService>();

        // Configura o mapper para converter Product -> ProductDto
        mockMapper
            .Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
            .Returns((Product p) => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            });

        var handler = new CreateProductCommandHandler(
            mockRepository.Object,
            mockMapper.Object,
            mockDiscountService.Object
        );

        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Price = 99.99m
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Name.Should().Be("Test Product");
        result.Price.Should().Be(99.99m);

        // Verifica se o repositório recebeu a chamada AddAsync
        mockRepository.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMapCorrectly_WhenRepositorySucceeds()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Product>>();
        var mockMapper = new Mock<IMapper>();
        var mockDiscountService = new Mock<DiscountService>();

        mockMapper
            .Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
            .Returns((Product p) => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price
            });

        var handler = new CreateProductCommandHandler(
            mockRepo.Object, mockMapper.Object, mockDiscountService.Object
        );

        var command = new CreateProductCommand { Name = "Test", Price = 10m };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Test");
        result.Price.Should().Be(10m);
    }

    [Fact]
    public async Task Handle_ShouldApplyDiscount_WhenPriceIsOverThreshold()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Product>>();
        var mockMapper = new Mock<IMapper>();
        var mockDiscount = new Mock<IDiscountService>();

        // Configuramos o mock para retornar 400 quando o preço for 600
        mockDiscount
            .Setup(d => d.ApplyConditionalDiscount(600))
            .Returns(400);

        var handler = new CreateProductCommandHandler(
            mockRepo.Object,
            mockMapper.Object,
            mockDiscount.Object
        );

        var command = new CreateProductCommand { Name = "Expensive Product", Price = 600m };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        mockDiscount.Verify(d => d.ApplyConditionalDiscount(600), Times.Once);
        mockRepo.Verify(r => r.AddAsync(It.Is<Product>(p => p.Price == 400)), Times.Once);
    }

}
