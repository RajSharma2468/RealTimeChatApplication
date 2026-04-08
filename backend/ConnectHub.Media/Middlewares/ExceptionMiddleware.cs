using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ConnectHub.Media.Middlewares
{
    // Global exception handler - catches all unhandled errors
    // WHY: Centralized error handling, no try-catch in every controller
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
                _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }
        
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            int statusCode;
            string message;
            
            // Different status codes for different exception types
            if (exception.Message.Contains("not found"))
            {
                statusCode = (int)HttpStatusCode.NotFound;
                message = exception.Message;
            }
            else if (exception.Message.Contains("size exceeds") || 
                     exception.Message.Contains("No file") ||
                     exception.Message.Contains("extension"))
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exception.Message.Contains("only delete your own"))
            {
                statusCode = (int)HttpStatusCode.Forbidden;
                message = exception.Message;
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An internal error occurred. Please try again.";
            }
            
            context.Response.StatusCode = statusCode;
            
            var response = new
            {
                success = false,
                message = message,
                statusCode = statusCode,
                timestamp = DateTime.UtcNow
            };
            
            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}