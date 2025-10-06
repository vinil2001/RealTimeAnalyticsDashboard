using System.Collections.Concurrent;
using SensorAnalyticsAPI.Models;

namespace SensorAnalyticsAPI.Services
{
    public interface ISensorDataService
    {
        void AddReading(SensorReading reading);
        IEnumerable<SensorReading> GetRecentReadings(int count = 1000);
        IEnumerable<SensorReading> GetReadingsBySensor(string sensorId, int count = 100);
        SensorStatistics GetSensorStatistics(string sensorId);
        IEnumerable<SensorStatistics> GetAllSensorStatistics();
        IEnumerable<AnomalyAlert> GetRecentAlerts(int count = 50);
        int GetTotalReadingsCount();
        void PurgeOldData();
    }

    public class SensorDataService : ISensorDataService
    {
        private readonly ConcurrentQueue<SensorReading> _readings = new();
        private readonly ConcurrentDictionary<string, ConcurrentQueue<SensorReading>> _sensorReadings = new();
        private readonly ConcurrentQueue<AnomalyAlert> _alerts = new();
        private readonly object _lockObject = new();
        private readonly ILogger<SensorDataService> _logger;

        // Performance optimization: Keep running statistics to avoid recalculation
        private readonly ConcurrentDictionary<string, RunningStatistics> _runningStats = new();

        public SensorDataService(ILogger<SensorDataService> logger)
        {
            _logger = logger;
        }

        public void AddReading(SensorReading reading)
        {
            // Add to main queue
            _readings.Enqueue(reading);

            // Add to sensor-specific queue
            _sensorReadings.AddOrUpdate(reading.SensorId,
                new ConcurrentQueue<SensorReading>(new[] { reading }),
                (key, existingQueue) =>
                {
                    existingQueue.Enqueue(reading);
                    return existingQueue;
                });

            // Update running statistics
            UpdateRunningStatistics(reading);

            // Check for anomalies
            CheckForAnomalies(reading);

            // Maintain memory limits (keep only last 100,000 readings)
            MaintainMemoryLimits();
        }

        public IEnumerable<SensorReading> GetRecentReadings(int count = 1000)
        {
            return _readings.TakeLast(count).OrderByDescending(r => r.Timestamp);
        }

        public IEnumerable<SensorReading> GetReadingsBySensor(string sensorId, int count = 100)
        {
            if (_sensorReadings.TryGetValue(sensorId, out var sensorQueue))
            {
                return sensorQueue.TakeLast(count).OrderByDescending(r => r.Timestamp);
            }
            return Enumerable.Empty<SensorReading>();
        }

        public SensorStatistics GetSensorStatistics(string sensorId)
        {
            if (_runningStats.TryGetValue(sensorId, out var stats))
            {
                return new SensorStatistics
                {
                    SensorId = sensorId,
                    Type = stats.Type,
                    Average = stats.Sum / stats.Count,
                    Min = stats.Min,
                    Max = stats.Max,
                    StandardDeviation = Math.Sqrt((stats.SumOfSquares - (stats.Sum * stats.Sum / stats.Count)) / stats.Count),
                    Count = stats.Count,
                    LastUpdate = stats.LastUpdate
                };
            }

            return new SensorStatistics { SensorId = sensorId };
        }

        public IEnumerable<SensorStatistics> GetAllSensorStatistics()
        {
            return _runningStats.Keys.Select(GetSensorStatistics);
        }

        public IEnumerable<AnomalyAlert> GetRecentAlerts(int count = 50)
        {
            return _alerts.TakeLast(count).OrderByDescending(a => a.Timestamp);
        }

        public int GetTotalReadingsCount()
        {
            return _readings.Count;
        }

        public void PurgeOldData()
        {
            var cutoffTime = DateTime.UtcNow.AddHours(-24);
            
            // Purge main readings
            var readingsToKeep = new List<SensorReading>();
            while (_readings.TryDequeue(out var reading))
            {
                if (reading.Timestamp > cutoffTime)
                {
                    readingsToKeep.Add(reading);
                }
            }

            foreach (var reading in readingsToKeep)
            {
                _readings.Enqueue(reading);
            }

            // Purge sensor-specific readings
            foreach (var kvp in _sensorReadings)
            {
                var sensorQueue = kvp.Value;
                var sensorReadingsToKeep = new List<SensorReading>();
                
                while (sensorQueue.TryDequeue(out var reading))
                {
                    if (reading.Timestamp > cutoffTime)
                    {
                        sensorReadingsToKeep.Add(reading);
                    }
                }

                foreach (var reading in sensorReadingsToKeep)
                {
                    sensorQueue.Enqueue(reading);
                }
            }

            // Purge old alerts
            var alertsToKeep = new List<AnomalyAlert>();
            while (_alerts.TryDequeue(out var alert))
            {
                if (alert.Timestamp > cutoffTime)
                {
                    alertsToKeep.Add(alert);
                }
            }

            foreach (var alert in alertsToKeep)
            {
                _alerts.Enqueue(alert);
            }

            _logger.LogInformation($"Purged data older than 24 hours. Current readings count: {_readings.Count}");
        }

        private void UpdateRunningStatistics(SensorReading reading)
        {
            _runningStats.AddOrUpdate(reading.SensorId,
                new RunningStatistics
                {
                    Type = reading.Type,
                    Count = 1,
                    Sum = reading.Value,
                    SumOfSquares = reading.Value * reading.Value,
                    Min = reading.Value,
                    Max = reading.Value,
                    LastUpdate = reading.Timestamp
                },
                (key, existing) =>
                {
                    existing.Count++;
                    existing.Sum += reading.Value;
                    existing.SumOfSquares += reading.Value * reading.Value;
                    existing.Min = Math.Min(existing.Min, reading.Value);
                    existing.Max = Math.Max(existing.Max, reading.Value);
                    existing.LastUpdate = reading.Timestamp;
                    return existing;
                });
        }

        private void CheckForAnomalies(SensorReading reading)
        {
            if (_runningStats.TryGetValue(reading.SensorId, out var stats) && stats.Count > 10)
            {
                var mean = stats.Sum / stats.Count;
                var stdDev = Math.Sqrt((stats.SumOfSquares - (stats.Sum * stats.Sum / stats.Count)) / stats.Count);
                
                // Simple anomaly detection: value is more than 3 standard deviations from mean
                var threshold = 3 * stdDev;
                var deviation = Math.Abs(reading.Value - mean);

                if (deviation > threshold)
                {
                    var alert = new AnomalyAlert
                    {
                        SensorId = reading.SensorId,
                        SensorType = reading.Type,
                        Value = reading.Value,
                        Threshold = threshold,
                        Message = $"Sensor {reading.SensorId} reading {reading.Value:F2} deviates {deviation:F2} from mean {mean:F2} (threshold: {threshold:F2})",
                        Timestamp = reading.Timestamp,
                        Severity = deviation > threshold * 2 ? AlertSeverity.Critical : 
                                  deviation > threshold * 1.5 ? AlertSeverity.High : AlertSeverity.Medium
                    };

                    _alerts.Enqueue(alert);
                    _logger.LogWarning($"Anomaly detected: {alert.Message}");
                }
            }
        }

        private void MaintainMemoryLimits()
        {
            const int maxReadings = 100000;
            
            if (_readings.Count > maxReadings)
            {
                // Remove oldest 10% when limit is exceeded
                var toRemove = maxReadings / 10;
                for (int i = 0; i < toRemove && _readings.TryDequeue(out _); i++)
                {
                    // Just dequeue to remove
                }
            }
        }

        private class RunningStatistics
        {
            public SensorType Type { get; set; }
            public int Count { get; set; }
            public double Sum { get; set; }
            public double SumOfSquares { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }
            public DateTime LastUpdate { get; set; }
        }
    }
}
