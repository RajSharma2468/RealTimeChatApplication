using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ConnectHub.Auth.Middlewares
{
    public class RequestLoggingMiddleware
    {
        // Dependency Injection

        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Log request
            _logger.LogInformation("Request: {Method} {Path}", 
                context.Request.Method, 
                context.Request.Path);

            await _next(context);

            stopwatch.Stop();

            // Log response
            _logger.LogInformation("Response: {Method} {Path} - {StatusCode} ({ElapsedMs}ms)",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}