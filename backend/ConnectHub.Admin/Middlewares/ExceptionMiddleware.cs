using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ConnectHub.Admin.Middlewares
{
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
            
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "An internal error occurred";
            
            if (exception.Message.Contains("not found"))
                statusCode = (int)HttpStatusCode.NotFound;
            else if (exception.Message.Contains("unauthorized"))
                statusCode = (int)HttpStatusCode.Unauthorized;
            else if (exception.Message.Contains("forbidden"))
                statusCode = (int)HttpStatusCode.Forbidden;
            
            context.Response.StatusCode = statusCode;
            
            var response = new { success = false, message = exception.Message, statusCode = statusCode };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}