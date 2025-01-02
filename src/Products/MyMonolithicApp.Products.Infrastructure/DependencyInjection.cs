using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Products.Infrastructure.Data;
using MyMonolithicApp.Products.Infrastructure.Repositories;

namespace MyMonolithicApp.Products.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProductsInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ProductsDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IRepository<Product>, ProductRepository>();

            return services;
        }
    }
}
