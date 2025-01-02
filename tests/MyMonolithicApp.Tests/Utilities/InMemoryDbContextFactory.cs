using Microsoft.EntityFrameworkCore;

namespace MyMonolithicApp.Tests.Utilities
{
    public class InMemoryDbContextFactory<TContext> : IDisposable
        where TContext : DbContext
    {
        private readonly DbContextOptions<TContext> _options;
        public TContext Context { get; private set; }

        public InMemoryDbContextFactory()
        {
            // Nome aleatório de banco em memória para isolar cada teste
            var dbName = $"{typeof(TContext).Name}_{Guid.NewGuid()}";

            _options = new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            // Cria a instância do DbContext via reflection (exige construtor que receba DbContextOptions<TContext>)
            Context = Activator.CreateInstance(typeof(TContext), _options) as TContext;
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
