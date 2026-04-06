using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ConnectHub.Room.Middlewares
{
    // Request Logging Middleware
    // WHY: Logs all incoming requests and outgoing responses for debugging
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
            
            // Store response body for logging
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            
            try
            {
                // Process the request
                await _next(context);
                
                stopwatch.Stop();
                
                // Log response
                _logger.LogInformation("← Response: {Method} {Path} - {StatusCode} ({ElapsedMs}ms)",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            finally
            {
                // Copy response body back
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            }
        }
    }
}