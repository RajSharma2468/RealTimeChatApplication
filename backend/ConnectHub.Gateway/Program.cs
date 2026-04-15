using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.OpenApi.Models;

// ================================================================
// BUILD THE APPLICATION
// ================================================================
var builder = WebApplication.CreateBuilder(args);

// ================================================================
// 1. LOAD OCELOT CONFIGURATION
// ================================================================
// Ocelot.json contains all routing rules for microservices
// Each route maps an incoming request to the appropriate service
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

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
// to communicate with gateway (port 5031)
//
// IMPORTANT: SignalR WebSockets require:
// - Specific origin (not wildcard *)
// - AllowCredentials() for authentication
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")  // React development server
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