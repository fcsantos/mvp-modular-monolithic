using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MyMonolithicApp.Products.Infrastructure.Data;
using System.Linq;

namespace IntegrationTests.IntegrationTests
{
  public class CustomWebApplicationFactory : WebApplicationFactory<Program>
  {
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.ConfigureServices(services =>
      {
        // Remove a configuração do DbContext com SQL
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<ProductsDbContext>));
        if (descriptor != null)
          services.Remove(descriptor);

        // Adiciona o DbContext in memory
        services.AddDbContext<ProductsDbContext>(options =>
        {
          options.UseInMemoryDatabase("IntegrationTestsDb");
        });
      });
    }
  }
}
