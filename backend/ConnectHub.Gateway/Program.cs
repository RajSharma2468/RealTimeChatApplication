using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Load Ocelot Configuration
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add Ocelot Services
builder.Services.AddOcelot();

// Add Swagger
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
// CORS CONFIGURATION - FIXED (Local + Render)
// ================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",                          // Local React
                "https://connecthub-frontend-x4xm.onrender.com"   // Render Frontend
              )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConnectHub API Gateway v1");
    });
}

// CORS - Must be before Ocelot
app.UseCors("AllowFrontend");

// Ocelot - Routes requests to microservices
await app.UseOcelot();

app.Run();