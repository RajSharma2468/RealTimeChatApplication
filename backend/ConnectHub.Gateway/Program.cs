using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.OpenApi.Models;

// ================================================================
// BUILD THE APPLICATION
// ================================================================
var builder = WebApplication.CreateBuilder(args);

// ================================================================
// CRITICAL FIX: Disable file watcher to avoid inotify limit on Render
// ================================================================
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: false);

// ================================================================
// 1. LOAD OCELOT CONFIGURATION
// ================================================================
// Ocelot.json contains all routing rules for microservices
// Each route maps an incoming request to the appropriate service
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: false);

// ================================================================
// 2. ADD OCELOT SERVICES
// ================================================================
// Ocelot handles request routing, load balancing, and middleware
builder.Services.AddOcelot();

// ================================================================
// 3. ADD SWAGGER FOR API DOCUMENTATION
// ================================================================
// Provides interactive API documentation at /swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "ConnectHub API Gateway", 
        Version = "v1" 
    });
});

// ================================================================
// 4. CORS CONFIGURATION - Allow React Frontend
// ================================================================
// CORS (Cross-Origin Resource Sharing) allows frontend (port 3000)
// to communicate with gateway (port 5000)
//
// IMPORTANT: SignalR WebSockets require:
// - Specific origin (not wildcard *)
// - AllowCredentials() for authentication
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://connecthub-frontend.onrender.com")
              .AllowAnyMethod()                      // Allow GET, POST, PUT, DELETE
              .AllowAnyHeader()                      // Allow any headers
              .AllowCredentials();                   // Required for SignalR WebSockets
    });
});

// ================================================================
// 5. BUILD THE APP
// ================================================================
var app = builder.Build();

// ================================================================
// 6. MIDDLEWARE PIPELINE
// ================================================================

// Handle OPTIONS preflight requests
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        context.Response.Headers.Append("Access-Control-Allow-Methods", "*");
        context.Response.Headers.Append("Access-Control-Allow-Headers", "*");
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});

// Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConnectHub API Gateway v1");
    });
}

// CORS - Must be called before Ocelot
app.UseCors("AllowFrontend");

// Ocelot - Routes requests to microservices
await app.UseOcelot();

// Run the application
app.Run();