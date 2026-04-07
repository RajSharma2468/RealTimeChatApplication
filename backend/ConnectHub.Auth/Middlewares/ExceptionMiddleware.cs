using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ConnectHub.Auth.Middlewares
{
    public class ExceptionMiddleware
    {
        // Dependency Injection

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new
            {
                success = false,
                message = exception.Message,
                statusCode = (int)HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            if (exception.Message.Contains("Username") || exception.Message.Contains("Email"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new { success = false, message = exception.Message, statusCode = 400 };
            }
            else if (exception.Message.Contains("password"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response = new { success = false, message = exception.Message, statusCode = 401 };
            }

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}