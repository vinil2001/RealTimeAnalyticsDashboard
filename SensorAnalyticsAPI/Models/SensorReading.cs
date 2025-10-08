namespace SensorAnalyticsAPI.Models
{
    public class SensorReading
    {
        public string SensorId { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public SensorType Type { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public enum SensorType
    {
        Temperature,
        Humidity,
        Pressure,
        Light,
        Motion,
        Sound,
        AirQuality
    }

    public class SensorStatistics
    {
        public string SensorId { get; set; } = string.Empty;
        public SensorType Type { get; set; }
        public double AverageValue { get; set; }  // Renamed for consistency with database context
        public double MinValue { get; set; }      // Renamed for consistency with database context
        public double MaxValue { get; set; }      // Renamed for consistency with database context
        public double StandardDeviation { get; set; }
        public int Count { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class AnomalyAlert
    {
        public int Id { get; set; }  // Changed to int for auto-increment primary key
        public string SensorId { get; set; } = string.Empty;
        public SensorType Type { get; set; }  // Renamed for consistency
        public double Value { get; set; }
        public double ExpectedValue { get; set; }  // Added for better anomaly tracking
        public double Deviation { get; set; }      // Added for deviation amount
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public int Severity { get; set; }  // Changed to int (0=Low, 1=Medium, 2=High, 3=Critical)
    }

    public enum AlertSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }
}
