using AutoMapper;
using FluentAssertions;
using Moq;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Exceptions;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Products.Application.Commands;
using MyMonolithicApp.Products.Application.DTOs;
using MyMonolithicApp.Products.Application.Services;

namespace MyMonolithicApp.Tests.Products.Application.Handlers;

public class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Product>>();
        var mockMapper = new Mock<IMapper>();
        var mockDiscount = new Mock<IDiscountService>();
        var handler = new UpdateProductCommandHandler(mockRepo.Object, mockMapper.Object, mockDiscount.Object);

        var command = new UpdateProductCommand
        {
            Id = Guid.NewGuid(), // ID qualquer, não existe no repositório
            Name = "New Name",
            Price = 100m
        };

        // Simula que o repositório não encontrou o produto
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Product?)null);

        // Act
        Func<Task> act = async () => await handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("*not found*"); // Pode checar parte da mensagem
    }

    [Fact]
    public async Task Handle_ShouldUpdateProduct_WhenRepositorySucceeds()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Product>>();
        var mockMapper = new Mock<IMapper>();
        var mockDiscount = new Mock<IDiscountService>();

        var existingProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Price = 50m
        };

        mockRepo.Setup(r => r.GetByIdAsync(existingProduct.Id))
                .ReturnsAsync(existingProduct);

        mockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
          .Returns((Product source) => new ProductDto
          {
              Id = source.Id,
              Name = source.Name,
              Price = source.Price
          });

        mockDiscount.Setup(d => d.ApplyConditionalDiscount(It.IsAny<decimal>()))
                    .Returns((decimal price) => price); // Assuming no discount is applied

        var handler = new UpdateProductCommandHandler(mockRepo.Object, mockMapper.Object, mockDiscount.Object);
        var command = new UpdateProductCommand
        {
            Id = existingProduct.Id,
            Name = "New Name",
            Price = 99.99m
        };

        // Act
        var updatedDto = await handler.Handle(command, CancellationToken.None);

        // Assert
        updatedDto.Name.Should().Be("New Name");
        updatedDto.Price.Should().Be(99.99m);
        mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

}
