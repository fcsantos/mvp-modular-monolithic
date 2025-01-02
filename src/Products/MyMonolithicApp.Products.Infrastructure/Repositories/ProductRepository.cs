using Microsoft.EntityFrameworkCore;
using MyMonolithicApp.Core.Entities;
using MyMonolithicApp.Core.Interfaces;
using MyMonolithicApp.Products.Infrastructure.Data;

namespace MyMonolithicApp.Products.Infrastructure.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly ProductsDbContext _context;

        public ProductRepository(ProductsDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product entity)
        {
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products.ToListAsync();

        public async Task<Product> GetByIdAsync(Guid id) => await _context.Products.FindAsync(id);

        public async Task UpdateAsync(Product entity)
        {
            _context.Products.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
