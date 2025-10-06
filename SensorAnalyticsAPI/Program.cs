using SensorAnalyticsAPI.Hubs;
using SensorAnalyticsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddOpenApi();

// Add CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Register custom services
builder.Services.AddSingleton<ISensorDataService, SensorDataService>();
builder.Services.AddHostedService<SensorSimulationService>();
builder.Services.AddHostedService<RealTimeUpdateService>();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAngular");
app.UseHttpsRedirection();
app.UseRouting();

// Map controllers and SignalR hub
app.MapControllers();
app.MapHub<SensorDataHub>("/sensorHub");

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.Run();
