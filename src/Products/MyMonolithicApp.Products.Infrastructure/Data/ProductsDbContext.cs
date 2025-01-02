using Microsoft.EntityFrameworkCore;
using MyMonolithicApp.Core.Entities;

namespace MyMonolithicApp.Products.Infrastructure.Data
{
    public class ProductsDbContext : DbContext
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações extras de mapeamento...
            base.OnModelCreating(modelBuilder);


            // Opção A: Aplicar configurações individualmente
            // modelBuilder.ApplyConfiguration(new ProductConfiguration());

            // Opção B: Aplicar todas as classes que implementam IEntityTypeConfiguration<T>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductsDbContext).Assembly);
        }
    }
}
