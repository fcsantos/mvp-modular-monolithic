using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MyMonolithicApp.Core.Exceptions;
using System.Net;

namespace MyMonolithicApp.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Chama o próximo middleware ou endpoint
                await _next(context);
            }
            catch (NotFoundException ex)
            {
                await HandleNotFoundExceptionAsync(context, ex);
            }
            catch (BadRequestException ex)
            {
                await HandleBadRequestExceptionAsync(context, ex);
            }
            catch (ValidationException ex)
            {
                // Intercepta a ValidationException do FluentValidation
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                // Aqui interceptamos outras exceções, se quiser
                await HandleGenericExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Trata a NotFoundException e retorna 404.
        /// </summary>
        private static Task HandleNotFoundExceptionAsync(HttpContext context, NotFoundException ex)
        {
            var statusCode = (int)HttpStatusCode.NotFound;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Not Found",
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            return context.Response.WriteAsJsonAsync(problemDetails);
        }

        /// <summary>
        /// Trata a ValidationException do FluentValidation e retorna 400.
        /// </summary>
        private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException ex)
        {
            // Podemos retornar um payload seguindo o "RFC 7807" (ProblemDetails)
            // ou simplesmente um objeto com as mensagens.
            var statusCode = (int)HttpStatusCode.BadRequest;

            // Estruturamos cada erro (property, mensagem)
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            var problemDetails = new ValidationProblemDetails(errors)
            {
                Status = statusCode,
                Title = "Validation failed",
                Detail = "One or more validation errors occurred.",
                Instance = context.Request.Path
            };

            // Para retornar no formato default do ASP.NET (RFC 7807)
            // definimos o content type "application/problem+json"
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            return context.Response.WriteAsJsonAsync(problemDetails);
        }

        /// <summary>
        /// Handles the BadRequestException and returns a 400 status code with a problem details response.
        /// </summary>
        private static Task HandleBadRequestExceptionAsync(HttpContext context, BadRequestException ex)
        {
            var statusCode = (int)HttpStatusCode.BadRequest;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Bad Request",
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            return context.Response.WriteAsJsonAsync(problemDetails);
        }


        /// <summary>
        /// Trata outras exceções genéricas e retorna 500.
        /// </summary>
        private static Task HandleGenericExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = "Internal Server Error",
                Detail = ex.Message, // Em produção, cuidado ao expor detalhes
                Instance = context.Request.Path
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            return context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}