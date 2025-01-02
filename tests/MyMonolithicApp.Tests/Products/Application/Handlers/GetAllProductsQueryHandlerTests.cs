using AutoMapper;
using FluentAssertions;
using Moq;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Products.Application.DTOs;
using MyMonolithicApp.Products.Application.Queries;

namespace MyMonolithicApp.Tests.Products.Application.Handlers;

public class GetAllProductsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoProducts()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Product>>();
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>());

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
                  .Returns(new List<ProductDto>());

        var handler = new GetAllProductsQueryHandler(mockRepo.Object, mockMapper.Object);

        // Act
        var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfProducts_WhenProductsExist()
    {
        // Arrange
        var products = new List<Product>
    {
        new Product { Id = Guid.NewGuid(), Name = "Prod1", Price = 10 },
        new Product { Id = Guid.NewGuid(), Name = "Prod2", Price = 20 }
    };

        var mockRepo = new Mock<IRepository<Product>>();
        mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(products))
                  .Returns(new List<ProductDto>
                  {
                  new ProductDto { Name = "Prod1", Price = 10 },
                  new ProductDto { Name = "Prod2", Price = 20 }
                  });

        var handler = new GetAllProductsQueryHandler(mockRepo.Object, mockMapper.Object);

        // Act
        var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Prod1");
    }

}
