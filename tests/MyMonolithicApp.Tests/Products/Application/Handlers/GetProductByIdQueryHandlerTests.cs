using AutoMapper;
using FluentAssertions;
using Moq;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Exceptions;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Products.Application.DTOs;
using MyMonolithicApp.Products.Application.Queries;

namespace MyMonolithicApp.Tests.Products.Application.Handlers;

public class GetProductByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Product>>();
        var mockMapper = new Mock<IMapper>();
        var handler = new GetProductByIdQueryHandler(mockRepo.Object, mockMapper.Object);
        var productId = Guid.NewGuid();

        // Simula que o repositório não encontrou o produto
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Product?)null);

        // Act
        Func<Task> act = async () => await handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("*not found*"); // Pode checar parte da mensagem
    }

    [Fact]
    public async Task Handle_ShouldReturnProductDto_WhenProductExists()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Product>>();
        var mockMapper = new Mock<IMapper>();
        var handler = new GetProductByIdQueryHandler(mockRepo.Object, mockMapper.Object);
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Test Product" };
        var productDto = new ProductDto { Id = productId, Name = "Test Product" };

        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(product);
        mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                  .Returns(productDto);

        // Act
        var result = await handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(productDto);
    }
}
