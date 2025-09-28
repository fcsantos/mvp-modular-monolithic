using AutoMapper;
using FluentAssertions;
using Moq;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Exceptions;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Products.Application.Commands;
using MyMonolithicApp.Products.Application.DTOs;
using Xunit;

namespace MyMonolithicApp.Tests.Products.Application.Handlers
{
    public class UpdateProductStockCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldUpdateStock_WhenProductExists()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Test Product",
                Price = 100m,
                StockQuantity = 50
            };

            var mockRepository = new Mock<IRepository<Product>>();
            var mockMapper = new Mock<IMapper>();

            mockRepository
                .Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            mockMapper
                .Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns((Product p) => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    IsInStock = p.IsInStock,
                    IsAvailable = p.IsAvailable
                });

            var handler = new UpdateProductStockCommandHandler(mockRepository.Object, mockMapper.Object);
            var command = new UpdateProductStockCommand(productId, 100);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.StockQuantity.Should().Be(100);
            existingProduct.StockQuantity.Should().Be(100);
            mockRepository.Verify(r => r.UpdateAsync(existingProduct), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var mockRepository = new Mock<IRepository<Product>>();
            var mockMapper = new Mock<IMapper>();

            mockRepository
                .Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync((Product?)null);

            var handler = new UpdateProductStockCommandHandler(mockRepository.Object, mockMapper.Object);
            var command = new UpdateProductStockCommand(productId, 100);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrowArgumentException_WhenStockIsNegative()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Test Product",
                Price = 100m,
                StockQuantity = 50
            };

            var mockRepository = new Mock<IRepository<Product>>();
            var mockMapper = new Mock<IMapper>();

            mockRepository
                .Setup(r => r.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            var handler = new UpdateProductStockCommandHandler(mockRepository.Object, mockMapper.Object);
            var command = new UpdateProductStockCommand(productId, -10);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));
        }
    }
}