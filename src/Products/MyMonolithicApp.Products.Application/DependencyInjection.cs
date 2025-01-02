using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyMonolithicApp.Products.Application.Behaviors;
using MyMonolithicApp.Products.Application.Services;
using System.Reflection;

namespace MyMonolithicApp.Products.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProductsApplication(this IServiceCollection services)
        {
            // Registra todos os Handlers do MediatR
            //services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            // Registra mapeamentos do AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Registra todos os Validators do FluentValidation localizados neste assembly
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Adiciona nosso ValidationBehavior no pipeline do MediatR
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            services.AddTransient<IDiscountService, DiscountService>();

            return services;
        }
    }
}
