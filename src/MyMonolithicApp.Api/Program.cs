using Serilog;
using MyMonolithicApp.Products.Application;
using MyMonolithicApp.Products.Infrastructure;
using MyMonolithicApp.Api.Middleware;
using MyMonolithicApp.Products.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Serilog
// OBS: Ao inves de criar um arquivo, pode incluir em uma BD NoSQL ou Elasticsearch ou CloudWatch ou outro destino
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// 2. Adicionar serviços
builder.Services.AddControllers();

// 3. Registrar ProductsApplication + ProductsInfrastructure
string? productsConnection = builder.Configuration.GetConnectionString("ProductsConnection");
if (string.IsNullOrEmpty(productsConnection))
{
    throw new InvalidOperationException("Connection string 'ProductsConnection' is not configured.");
}

builder.Services.AddProductsApplication();
builder.Services.AddProductsInfrastructure(productsConnection);

// 4. (Opcional) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 5. (Opcional) Criar ou migrar o banco de dados

// Importante:
//Em produção, é comum ter um processo de CI/CD ou script de deploy que aplica as migrações antes de iniciar a aplicação.
//Mas para projetos simples, labs ou cenários “dev-friendly ou MVP”, fazer o dbContext.Database.Migrate()
//no Program.cs é prático.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ProductsDbContext>();
        dbContext.Database.Migrate();
        //dbContext.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        // Você pode registrar o erro com Serilog ou outra ferramenta de logging
        Log.Fatal(ex, "Erro ao criar/aplicar migrações do banco de dados.");
        throw; // Opcional: relançar a exceção para encerrar a aplicação
    }
}

// 6. Registrar nosso middleware:
app.UseMiddleware<GlobalExceptionMiddleware>();

// 7. Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();