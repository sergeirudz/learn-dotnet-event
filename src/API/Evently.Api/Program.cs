using Evently.Api.Extensions;
using Evently.Common.Application;
using Evently.Common.Infrastructure;
using Evently.Modules.Events.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

// Eception handling middleware
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(t => t.FullName?.Replace("+", "."));
});

// This way any modules can be registered without the need to reference. 
// src/Common/Evently.Common.Application/ApplicationConfiguration.cs
builder.Services.AddApplication([Evently.Modules.Events.Application.AssemblyReference.Assembly]);

// Register src/Common/Evently.Common.Infrastructure/InfrastructureConfiguration.cs
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("Database")!,
    builder.Configuration.GetConnectionString("Cache")!);

// Add appsettings.json
// src/API/Evently.Api/Extensions/ConfigurationExtensions.cs
builder.Configuration.AddModuleConfiguration(["events"]);

builder.Services.AddHealthChecks()
    .AddNpgSql(databaseConnectionString) // HealthCheck for PostgreSQL database
    .AddRedis(redisConnectionString); // HealthCheck for Redis cache

builder.Services.AddEventsModule(builder.Configuration);

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

EventsModule.MapEndpoints(app);

app.MapHealthChecks("health", new HealthCheckOptions // create health check endpoint at /health
{
    // AspNetCore.HealthChecks.UI.Client 
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Logging middleware, track incoming API requests and log them with Serilog
UseSerilogRequestLogging
    
app.UseExceptionHandler();

app.Run();
