using Microsoft.EntityFrameworkCore;
using SensorAnalyticsAPI.Data;
using SensorAnalyticsAPI.Models;
using System.Collections.Concurrent;

namespace SensorAnalyticsAPI.Services
{
    public class DatabaseSensorDataService : ISensorDataService
    {
        private readonly SensorContext _context;
        private readonly ILogger<DatabaseSensorDataService> _logger;
        
        // Keep some in-memory cache for performance
        private readonly ConcurrentQueue<SensorReading> _recentReadingsCache = new();
        private readonly ConcurrentQueue<AnomalyAlert> _recentAlertsCache = new();
        private DateTime _lastCacheUpdate = DateTime.MinValue;

        public DatabaseSensorDataService(SensorContext context, ILogger<DatabaseSensorDataService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void AddReading(SensorReading reading)
        {
            try
            {
                // Add to database
                _context.SensorReadings.Add(reading);
                _context.SaveChanges();

                // Add to cache for real-time performance
                _recentReadingsCache.Enqueue(reading);
                
                // Keep cache size manageable
                while (_recentReadingsCache.Count > 1000)
                {
                    _recentReadingsCache.TryDequeue(out _);
                }

                // Update statistics
                UpdateSensorStatistics(reading);

                // Check for anomalies
                CheckForAnomalies(reading);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding sensor reading to database");
                throw;
            }
        }

        public IEnumerable<SensorReading> GetRecentReadings(int count = 1000)
        {
            try
            {
                // Use cache if it's recent enough (within 5 seconds)
                if (DateTime.UtcNow - _lastCacheUpdate < TimeSpan.FromSeconds(5) && _recentReadingsCache.Count > 0)
                {
                    return _recentReadingsCache.ToArray().TakeLast(count);
                }

                // Otherwise query database
                var readings = _context.SensorReadings
                    .OrderByDescending(r => r.Timestamp)
                    .Take(count)
                    .ToList();

                // Update cache
                _recentReadingsCache.Clear();
                foreach (var reading in readings.OrderBy(r => r.Timestamp))
                {
                    _recentReadingsCache.Enqueue(reading);
                }
                _lastCacheUpdate = DateTime.UtcNow;

                return readings.OrderBy(r => r.Timestamp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent readings from database");
                return Enumerable.Empty<SensorReading>();
            }
        }

        public IEnumerable<SensorReading> GetReadingsBySensor(string sensorId, int count = 100)
        {
            try
            {
                return _context.SensorReadings
                    .Where(r => r.SensorId == sensorId)
                    .OrderByDescending(r => r.Timestamp)
                    .Take(count)
                    .OrderBy(r => r.Timestamp)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting readings for sensor {SensorId}", sensorId);
                return Enumerable.Empty<SensorReading>();
            }
        }

        public SensorStatistics GetSensorStatistics(string sensorId)
        {
            try
            {
                var stats = _context.SensorStatistics
                    .FirstOrDefault(s => s.SensorId == sensorId);

                if (stats == null)
                {
                    // Calculate from readings if no cached stats
                    var readings = _context.SensorReadings
                        .Where(r => r.SensorId == sensorId)
                        .ToList();

                    if (readings.Any())
                    {
                        stats = CalculateStatistics(sensorId, readings);
                        _context.SensorStatistics.Add(stats);
                        _context.SaveChanges();
                    }
                }

                return stats ?? new SensorStatistics { SensorId = sensorId };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting statistics for sensor {SensorId}", sensorId);
                return new SensorStatistics { SensorId = sensorId };
            }
        }

        public IEnumerable<SensorStatistics> GetAllSensorStatistics()
        {
            try
            {
                return _context.SensorStatistics.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all sensor statistics");
                return Enumerable.Empty<SensorStatistics>();
            }
        }

        public IEnumerable<AnomalyAlert> GetRecentAlerts(int count = 50)
        {
            try
            {
                // Use cache for recent alerts
                if (_recentAlertsCache.Count > 0)
                {
                    return _recentAlertsCache.ToArray().TakeLast(count);
                }

                // Query database
                var alerts = _context.AnomalyAlerts
                    .OrderByDescending(a => a.Timestamp)
                    .Take(count)
                    .OrderBy(a => a.Timestamp)
                    .ToList();

                return alerts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent alerts");
                return Enumerable.Empty<AnomalyAlert>();
            }
        }

        public int GetTotalReadingsCount()
        {
            try
            {
                return _context.SensorReadings.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total readings count");
                return 0;
            }
        }

        public void PurgeOldData()
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddHours(-24);
                
                // Remove old readings
                var oldReadings = _context.SensorReadings
                    .Where(r => r.Timestamp < cutoffDate);
                _context.SensorReadings.RemoveRange(oldReadings);

                // Remove old alerts
                var oldAlerts = _context.AnomalyAlerts
                    .Where(a => a.Timestamp < cutoffDate);
                _context.AnomalyAlerts.RemoveRange(oldAlerts);

                _context.SaveChanges();
                
                _logger.LogInformation("Purged data older than {CutoffDate}", cutoffDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error purging old data");
            }
        }

        private void UpdateSensorStatistics(SensorReading reading)
        {
            try
            {
                var stats = _context.SensorStatistics
                    .FirstOrDefault(s => s.SensorId == reading.SensorId && s.Type == reading.Type);

                if (stats == null)
                {
                    stats = new SensorStatistics
                    {
                        SensorId = reading.SensorId,
                        Type = reading.Type,
                        AverageValue = reading.Value,
                        MinValue = reading.Value,
                        MaxValue = reading.Value,
                        StandardDeviation = 0,
                        Count = 1,
                        LastUpdate = reading.Timestamp
                    };
                    _context.SensorStatistics.Add(stats);
                }
                else
                {
                    // Update running statistics
                    var newCount = stats.Count + 1;
                    var newAverage = (stats.AverageValue * stats.Count + reading.Value) / newCount;
                    
                    stats.AverageValue = newAverage;
                    stats.MinValue = Math.Min(stats.MinValue, reading.Value);
                    stats.MaxValue = Math.Max(stats.MaxValue, reading.Value);
                    stats.Count = newCount;
                    stats.LastUpdate = reading.Timestamp;
                    
                    // Recalculate standard deviation periodically
                    if (newCount % 100 == 0)
                    {
                        RecalculateStandardDeviation(stats);
                    }
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sensor statistics");
            }
        }

        private void RecalculateStandardDeviation(SensorStatistics stats)
        {
            var readings = _context.SensorReadings
                .Where(r => r.SensorId == stats.SensorId && r.Type == stats.Type)
                .Select(r => r.Value)
                .ToList();

            if (readings.Count > 1)
            {
                var mean = readings.Average();
                var sumOfSquares = readings.Sum(x => Math.Pow(x - mean, 2));
                stats.StandardDeviation = Math.Sqrt(sumOfSquares / (readings.Count - 1));
            }
        }

        private void CheckForAnomalies(SensorReading reading)
        {
            try
            {
                var stats = _context.SensorStatistics
                    .FirstOrDefault(s => s.SensorId == reading.SensorId && s.Type == reading.Type);

                if (stats != null && stats.Count > 10 && stats.StandardDeviation > 0)
                {
                    var deviation = Math.Abs(reading.Value - stats.AverageValue);
                    var threshold = 3 * stats.StandardDeviation; // 3-sigma rule

                    if (deviation > threshold)
                    {
                        var alert = new AnomalyAlert
                        {
                            SensorId = reading.SensorId,
                            Type = reading.Type,
                            Value = reading.Value,
                            ExpectedValue = stats.AverageValue,
                            Deviation = deviation,
                            Timestamp = reading.Timestamp,
                            Severity = CalculateSeverity(deviation, threshold),
                            Message = $"Sensor {reading.SensorId} reading {reading.Value:F2} deviates {deviation:F2} from mean {stats.AverageValue:F2} (threshold: {threshold:F2})"
                        };

                        _context.AnomalyAlerts.Add(alert);
                        _context.SaveChanges();

                        // Add to cache
                        _recentAlertsCache.Enqueue(alert);
                        while (_recentAlertsCache.Count > 100)
                        {
                            _recentAlertsCache.TryDequeue(out _);
                        }

                        _logger.LogWarning("Anomaly detected: {Message}", alert.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for anomalies");
            }
        }

        private static int CalculateSeverity(double deviation, double threshold)
        {
            var ratio = deviation / threshold;
            return ratio switch
            {
                < 1.5 => 0,  // Low
                < 2.0 => 1,  // Medium
                < 3.0 => 2,  // High
                _ => 3       // Critical
            };
        }

        private static SensorStatistics CalculateStatistics(string sensorId, List<SensorReading> readings)
        {
            if (!readings.Any()) return new SensorStatistics { SensorId = sensorId };

            var values = readings.Select(r => r.Value).ToList();
            var average = values.Average();
            var min = values.Min();
            var max = values.Max();
            var stdDev = values.Count > 1 
                ? Math.Sqrt(values.Sum(x => Math.Pow(x - average, 2)) / (values.Count - 1))
                : 0;

            return new SensorStatistics
            {
                SensorId = sensorId,
                Type = readings.First().Type,
                AverageValue = average,
                MinValue = min,
                MaxValue = max,
                StandardDeviation = stdDev,
                Count = readings.Count,
                LastUpdate = readings.Max(r => r.Timestamp)
            };
        }
    }
}
