using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Products.Infrastructure.Data;
using MyMonolithicApp.Products.Infrastructure.Repositories;
using MyMonolithicApp.Tests.Utilities;

namespace MyMonolithicApp.Tests.Products.Infrastructure.Repositories
{
    public class ProductRepositoryTests
    {
        [Fact]
        public async Task AddAsync_ShouldAddProductToInMemoryDatabase()
        {
            // Arrange
            using var factory = new InMemoryDbContextFactory<ProductsDbContext>();
            var context = factory.Context;

            IRepository<Product> repository = new ProductRepository(context);

            var product = new Product { Name = "Test Product", Price = 10m };

            // Act
            await repository.AddAsync(product);

            // Assert
            var dbProduct = await context.Products.FirstOrDefaultAsync();
            dbProduct.Should().NotBeNull();
            dbProduct?.Name.Should().Be("Test Product");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Arrange
            using var factory = new InMemoryDbContextFactory<ProductsDbContext>();
            var context = factory.Context;

            // Adiciona alguns itens manualmente
            context.Products.Add(new Product { Name = "Product A", Price = 20m });
            context.Products.Add(new Product { Name = "Product B", Price = 30m });
            await context.SaveChangesAsync();

            IRepository<Product> repository = new ProductRepository(context);

            // Act
            var products = await repository.GetAllAsync();

            // Assert
            products.Should().HaveCount(2);
            products.Any(p => p.Name == "Product A").Should().BeTrue();
            products.Any(p => p.Name == "Product B").Should().BeTrue();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            using var factory = new InMemoryDbContextFactory<ProductsDbContext>();
            var context = factory.Context;

            IRepository<Product> repository = new ProductRepository(context);

            // Act
            var result = await repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenFound()
        {
            // Arrange
            using var factory = new InMemoryDbContextFactory<ProductsDbContext>();
            var context = factory.Context;

            var existingId = Guid.NewGuid();
            var existingProduct = new Product
            {
                Id = existingId,
                Name = "Found Product",
                Price = 99.9m
            };

            context.Products.Add(existingProduct);
            await context.SaveChangesAsync();

            IRepository<Product> repository = new ProductRepository(context);

            // Act
            var result = await repository.GetByIdAsync(existingId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(existingId);
            result.Name.Should().Be("Found Product");
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyExistingProduct()
        {
            // Arrange
            using var factory = new InMemoryDbContextFactory<ProductsDbContext>();
            var context = factory.Context;

            var existing = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Old Name",
                Price = 10m
            };

            context.Products.Add(existing);
            await context.SaveChangesAsync();

            IRepository<Product> repository = new ProductRepository(context);

            // Act
            existing.Name = "Updated Name";
            existing.Price = 99.99m;
            await repository.UpdateAsync(existing);

            // Assert
            var dbProduct = await context.Products.FindAsync(existing.Id);
            dbProduct.Name.Should().Be("Updated Name");
            dbProduct.Price.Should().Be(99.99m);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct()
        {
            // Arrange
            using var factory = new InMemoryDbContextFactory<ProductsDbContext>();
            var context = factory.Context;

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Will be deleted"
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            IRepository<Product> repository = new ProductRepository(context);

            // Act
            await repository.DeleteAsync(product.Id);

            // Assert
            var deleted = await context.Products.FindAsync(product.Id);
            deleted.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotThrow_WhenProductDoesNotExist()
        {
            // Arrange
            using var factory = new InMemoryDbContextFactory<ProductsDbContext>();
            var context = factory.Context;

            IRepository<Product> repository = new ProductRepository(context);
            var nonExistentId = Guid.NewGuid();

            // Act
            // Apenas certifica que não gera erro ao tentar deletar algo não existente
            await repository.DeleteAsync(nonExistentId);

            // Assert
            var count = await context.Products.CountAsync();
            count.Should().Be(0); // não foi adicionado nada
        }
    }
}