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
        public double Average { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double StandardDeviation { get; set; }
        public int Count { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class AnomalyAlert
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string SensorId { get; set; } = string.Empty;
        public SensorType SensorType { get; set; }
        public double Value { get; set; }
        public double Threshold { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public AlertSeverity Severity { get; set; }
    }

    public enum AlertSeverity
    {
        Low,
        Medium,
        High,
        Critical
    }
}
