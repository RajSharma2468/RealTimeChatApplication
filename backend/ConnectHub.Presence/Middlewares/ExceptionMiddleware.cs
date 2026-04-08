using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ConnectHub.Presence.Middlewares
{
    // Global exception handler - catches all unhandled errors
    public class ExceptionMiddleware
    {
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
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }
        
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = exception.Message.Contains("not found") 
                ? (int)HttpStatusCode.NotFound 
                : (int)HttpStatusCode.InternalServerError;
            
            context.Response.StatusCode = statusCode;
            
            var response = new 
            { 
                success = false, 
                message = exception.Message,
                statusCode = statusCode
            };
            
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}