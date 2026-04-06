using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ConnectHub.Room.Middlewares
{
    // Global Exception Handling Middleware
    // WHY: Centralized error handling, avoids try-catch in every controller
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        
        // Constructor - Dependency Injection
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        // Invoke method - called for every HTTP request
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pass request to next middleware in pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                
                // Return formatted error response
                await HandleExceptionAsync(context, ex);
            }
        }
        
        // Format error response based on exception type
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            int statusCode;
            string message;
            
            // Different status codes for different exception types
            if (exception.Message.Contains("not found") || exception.Message.Contains("does not exist"))
            {
                statusCode = (int)HttpStatusCode.NotFound;
                message = exception.Message;
            }
            else if (exception.Message.Contains("already a member") || 
                     exception.Message.Contains("already exists") ||
                     exception.Message.Contains("required"))
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else if (exception.Message.Contains("Only room admin") || 
                     exception.Message.Contains("not authorized") ||
                     exception.Message.Contains("Only room creator"))
            {
                statusCode = (int)HttpStatusCode.Forbidden;
                message = exception.Message;
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "An internal server error occurred. Please try again later.";
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