using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ConnectHub.Notification.Middlewares
{
    // Request logging middleware - logs every request and response
    // WHY: Debugging, monitoring, performance tracking
    public class RequestLoggingMiddleware
    {
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
            
            // Log incoming request
            _logger.LogInformation("→ Request: {Method} {Path} from {IP}", 
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown");
            
            try
            {
                await _next(context);
                
                stopwatch.Stop();
                
                // Log response
                _logger.LogInformation("← Response: {Method} {Path} - {StatusCode} ({ElapsedMs}ms)",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            catch
            {
                stopwatch.Stop();
                _logger.LogError("✗ Error: {Method} {Path} - ({ElapsedMs}ms)",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
