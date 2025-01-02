using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Text;
using System.Text.Json;

namespace IntegrationTests.IntegrationTests.Products
{
    public class ProductsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProductsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            // Cria o HttpClient com a aplicação já configurada
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProducts_ShouldReturn200Ok()
        {
            // Act
            var response = await _client.GetAsync("/api/products");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task PostProducts_ShouldReturn201Created()
        {
            // Arrange
            var productPayload = new
            {
                name = "New Product",
                price = 120.00
            };

            var jsonString = JsonSerializer.Serialize(productPayload);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/products", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var responseBody = await response.Content.ReadAsStringAsync();
            // Você pode desserializar e verificar se criou corretamente
        }
    }
}