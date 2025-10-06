using SensorAnalyticsAPI.Models;

namespace SensorAnalyticsAPI.Services
{
    public class SensorSimulationService : BackgroundService
    {
        private readonly ISensorDataService _dataService;
        private readonly ILogger<SensorSimulationService> _logger;
        private readonly Random _random = new();
        
        // Simulate 50 different sensors
        private readonly List<SensorConfig> _sensorConfigs;

        public SensorSimulationService(ISensorDataService dataService, ILogger<SensorSimulationService> logger)
        {
            _dataService = dataService;
            _logger = logger;
            _sensorConfigs = GenerateSensorConfigs();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Sensor simulation service started");

            // Target: 100 readings per second (reduced for better visualization)
            // With 50 sensors, each sensor generates 2 readings per second
            var readingsPerSensorPerSecond = 2;
            var intervalMs = 1000 / readingsPerSensorPerSecond; // 500ms per reading per sensor

            var tasks = _sensorConfigs.Select(config => 
                SimulateSensor(config, intervalMs, stoppingToken)).ToArray();

            await Task.WhenAll(tasks);
        }

        private async Task SimulateSensor(SensorConfig config, int intervalMs, CancellationToken stoppingToken)
        {
            var lastValue = config.BaseValue;
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Generate realistic sensor reading with some drift and noise
                    var drift = (_random.NextDouble() - 0.5) * config.DriftRange;
                    var noise = (_random.NextDouble() - 0.5) * config.NoiseRange;
                    var newValue = lastValue + drift + noise;
                    
                    // Keep value within realistic bounds
                    newValue = Math.Max(config.MinValue, Math.Min(config.MaxValue, newValue));
                    
                    // Occasionally inject anomalies (1% chance)
                    if (_random.NextDouble() < 0.01)
                    {
                        var anomalyMultiplier = _random.NextDouble() < 0.5 ? 0.1 : 3.0;
                        newValue *= anomalyMultiplier;
                    }

                    var reading = new SensorReading
                    {
                        SensorId = config.SensorId,
                        Timestamp = DateTime.UtcNow,
                        Value = Math.Round(newValue, 2),
                        Unit = config.Unit,
                        Type = config.Type,
                        Latitude = config.Latitude,
                        Longitude = config.Longitude
                    };

                    _dataService.AddReading(reading);
                    lastValue = newValue;

                    await Task.Delay(intervalMs, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error simulating sensor {config.SensorId}");
                    await Task.Delay(1000, stoppingToken); // Wait before retrying
                }
            }
        }

        private List<SensorConfig> GenerateSensorConfigs()
        {
            var configs = new List<SensorConfig>();
            var locations = GenerateLocations();

            // Temperature sensors (10 sensors)
            for (int i = 1; i <= 10; i++)
            {
                configs.Add(new SensorConfig
                {
                    SensorId = $"TEMP_{i:D3}",
                    Type = SensorType.Temperature,
                    BaseValue = 20 + _random.NextDouble() * 10, // 20-30°C
                    MinValue = -10,
                    MaxValue = 50,
                    DriftRange = 0.5,
                    NoiseRange = 0.2,
                    Unit = "°C",
                    Latitude = locations[i - 1].Latitude,
                    Longitude = locations[i - 1].Longitude
                });
            }

            // Humidity sensors (10 sensors)
            for (int i = 1; i <= 10; i++)
            {
                configs.Add(new SensorConfig
                {
                    SensorId = $"HUM_{i:D3}",
                    Type = SensorType.Humidity,
                    BaseValue = 40 + _random.NextDouble() * 40, // 40-80%
                    MinValue = 0,
                    MaxValue = 100,
                    DriftRange = 2,
                    NoiseRange = 1,
                    Unit = "%",
                    Latitude = locations[i + 9].Latitude,
                    Longitude = locations[i + 9].Longitude
                });
            }

            // Pressure sensors (10 sensors)
            for (int i = 1; i <= 10; i++)
            {
                configs.Add(new SensorConfig
                {
                    SensorId = $"PRES_{i:D3}",
                    Type = SensorType.Pressure,
                    BaseValue = 1013 + _random.NextDouble() * 20, // 1013-1033 hPa
                    MinValue = 950,
                    MaxValue = 1050,
                    DriftRange = 1,
                    NoiseRange = 0.5,
                    Unit = "hPa",
                    Latitude = locations[i + 19].Latitude,
                    Longitude = locations[i + 19].Longitude
                });
            }

            // Light sensors (10 sensors)
            for (int i = 1; i <= 10; i++)
            {
                configs.Add(new SensorConfig
                {
                    SensorId = $"LIGHT_{i:D3}",
                    Type = SensorType.Light,
                    BaseValue = 200 + _random.NextDouble() * 800, // 200-1000 lux
                    MinValue = 0,
                    MaxValue = 2000,
                    DriftRange = 50,
                    NoiseRange = 20,
                    Unit = "lux",
                    Latitude = locations[i + 29].Latitude,
                    Longitude = locations[i + 29].Longitude
                });
            }

            // Air Quality sensors (10 sensors)
            for (int i = 1; i <= 10; i++)
            {
                configs.Add(new SensorConfig
                {
                    SensorId = $"AQ_{i:D3}",
                    Type = SensorType.AirQuality,
                    BaseValue = 50 + _random.NextDouble() * 100, // 50-150 AQI
                    MinValue = 0,
                    MaxValue = 500,
                    DriftRange = 5,
                    NoiseRange = 2,
                    Unit = "AQI",
                    Latitude = locations[i + 39].Latitude,
                    Longitude = locations[i + 39].Longitude
                });
            }

            return configs;
        }

        private List<(double Latitude, double Longitude)> GenerateLocations()
        {
            var locations = new List<(double, double)>();
            
            // Generate 50 random locations around major cities
            var cities = new[]
            {
                (40.7128, -74.0060), // New York
                (34.0522, -118.2437), // Los Angeles
                (41.8781, -87.6298), // Chicago
                (29.7604, -95.3698), // Houston
                (33.4484, -112.0740), // Phoenix
            };

            foreach (var city in cities)
            {
                for (int i = 0; i < 10; i++)
                {
                    var latOffset = (_random.NextDouble() - 0.5) * 0.1; // ±0.05 degrees
                    var lonOffset = (_random.NextDouble() - 0.5) * 0.1;
                    locations.Add((city.Item1 + latOffset, city.Item2 + lonOffset));
                }
            }

            return locations;
        }

        private class SensorConfig
        {
            public string SensorId { get; set; } = string.Empty;
            public SensorType Type { get; set; }
            public double BaseValue { get; set; }
            public double MinValue { get; set; }
            public double MaxValue { get; set; }
            public double DriftRange { get; set; }
            public double NoiseRange { get; set; }
            public string Unit { get; set; } = string.Empty;
            public double? Latitude { get; set; }
            public double? Longitude { get; set; }
        }
    }
}
