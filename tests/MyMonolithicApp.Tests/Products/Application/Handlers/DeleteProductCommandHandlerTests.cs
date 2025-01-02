using FluentAssertions;
using Moq;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Exceptions;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Products.Application.Commands;

namespace MyMonolithicApp.Tests.Products.Application.Handlers;

public class DeleteProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenProductDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Product>>();
        var handler = new DeleteProductCommandHandler(mockRepo.Object);

        var command = new DeleteProductCommand(Guid.NewGuid()); // ID qualquer, não existe no repositório

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
    public async Task Handle_ShouldDeleteProduct_WhenRepositorySucceeds()
    {
        // Arrange
        var mockRepo = new Mock<IRepository<Product>>();
        var handler = new DeleteProductCommandHandler(mockRepo.Object);

        var existingProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Product to Delete"
        };

        mockRepo.Setup(r => r.GetByIdAsync(existingProduct.Id))
                .ReturnsAsync(existingProduct);

        var command = new DeleteProductCommand(existingProduct.Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        // Verificar se DeleteAsync foi chamado com o ID correto
        mockRepo.Verify(r => r.DeleteAsync(existingProduct.Id), Times.Once);
    }


}
