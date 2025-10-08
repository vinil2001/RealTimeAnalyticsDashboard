using Microsoft.EntityFrameworkCore;
using SensorAnalyticsAPI.Data;
using SensorAnalyticsAPI.Hubs;
using SensorAnalyticsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var useDatabase = !string.IsNullOrEmpty(connectionString);

if (useDatabase)
{
    // Production: Use PostgreSQL
    builder.Services.AddDbContext<SensorContext>(options =>
        options.UseNpgsql(connectionString));
    builder.Services.AddScoped<ISensorDataService, DatabaseSensorDataService>();
}
else
{
    // Development: Use in-memory storage
    builder.Services.AddSingleton<ISensorDataService, SensorDataService>();
}

// Add CORS for Angular frontend
var frontendUrl = builder.Configuration["FRONTEND_URL"] ?? "http://localhost:4200";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(frontendUrl, "http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Register hosted services
builder.Services.AddHostedService<SensorSimulationService>();
builder.Services.AddHostedService<RealTimeUpdateService>();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Initialize database if using PostgreSQL
if (useDatabase)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<SensorContext>();
        try
        {
            context.Database.EnsureCreated();
            app.Logger.LogInformation("Database initialized successfully");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Error initializing database");
        }
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAngular");

// Don't redirect to HTTPS in production (Railway handles this)
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

// Map controllers and SignalR hub
app.MapControllers();
app.MapHub<SensorDataHub>("/sensorHub");

// Enhanced health check endpoint
app.MapGet("/health", () => new { 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    environment = app.Environment.EnvironmentName,
    database = useDatabase ? "PostgreSQL" : "In-Memory"
});

// Use PORT environment variable for Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
