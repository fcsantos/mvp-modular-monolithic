using Serilog;
using MyMonolithicApp.Products.Application;
using MyMonolithicApp.Products.Infrastructure;
using MyMonolithicApp.Api.Middleware;
using MyMonolithicApp.Products.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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

// 2. Adicionar servi�os
builder.Services.AddControllers();

// 3. Registrar ProductsApplication + ProductsInfrastructure + Health Checks
string? productsConnection = builder.Configuration.GetConnectionString("ProductsConnection");
if (string.IsNullOrEmpty(productsConnection))
{
    throw new InvalidOperationException("Connection string 'ProductsConnection' is not configured.");
}

builder.Services.AddProductsApplication();
builder.Services.AddProductsInfrastructure(productsConnection);

// 4. Configurar Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ProductsDbContext>()
    .AddCheck("self", () => HealthCheckResult.Healthy("Application is running"));

// 4. (Opcional) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5. Configurar CORS para desenvolvimento
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 6. (Opcional) Criar ou migrar o banco de dados

// Importante:
//Em produ��o, � comum ter um processo de CI/CD ou script de deploy que aplica as migra��es antes de iniciar a aplica��o.
//Mas para projetos simples, labs ou cen�rios �dev-friendly ou MVP�, fazer o dbContext.Database.Migrate()
//no Program.cs � pr�tico.
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
        // Voc� pode registrar o erro com Serilog ou outra ferramenta de logging
        Log.Fatal(ex, "Erro ao criar/aplicar migra��es do banco de dados.");
        throw; // Opcional: relan�ar a exce��o para encerrar a aplica��o
    }
}

// 7. Registrar nosso middleware:
app.UseMiddleware<GlobalExceptionMiddleware>();

// 8. Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("DevelopmentPolicy");
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();

// Health checks endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();