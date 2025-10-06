using Microsoft.AspNetCore.SignalR;
using SensorAnalyticsAPI.Hubs;
using SensorAnalyticsAPI.Models;

namespace SensorAnalyticsAPI.Services
{
    public class RealTimeUpdateService : BackgroundService
    {
        private readonly IHubContext<SensorDataHub> _hubContext;
        private readonly ISensorDataService _dataService;
        private readonly ILogger<RealTimeUpdateService> _logger;
        private readonly Queue<SensorReading> _pendingReadings = new();
        private readonly Queue<AnomalyAlert> _pendingAlerts = new();
        private readonly object _lockObject = new();
        private DateTime _lastStatisticsUpdate = DateTime.MinValue;

        public RealTimeUpdateService(
            IHubContext<SensorDataHub> hubContext,
            ISensorDataService dataService,
            ILogger<RealTimeUpdateService> logger)
        {
            _hubContext = hubContext;
            _dataService = dataService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Real-time update service started");

            // Start data purge task
            var purgeTask = StartDataPurgeTask(stoppingToken);

            // Start real-time broadcast task
            var broadcastTask = StartBroadcastTask(stoppingToken);

            await Task.WhenAll(purgeTask, broadcastTask);
        }

        private async Task StartBroadcastTask(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Broadcast recent readings every 1000ms (1 time per second) - slower for better visualization
                    var recentReadings = _dataService.GetRecentReadings(20);
                    if (recentReadings.Any())
                    {
                        await _hubContext.Clients.All.SendAsync("NewReadings", recentReadings, stoppingToken);
                    }

                    // Broadcast statistics every 10 seconds
                    if (DateTime.UtcNow - _lastStatisticsUpdate > TimeSpan.FromSeconds(10))
                    {
                        var statistics = _dataService.GetAllSensorStatistics();
                        var totalCount = _dataService.GetTotalReadingsCount();
                        
                        await _hubContext.Clients.All.SendAsync("StatisticsUpdate", new
                        {
                            statistics = statistics,
                            totalCount = totalCount,
                            timestamp = DateTime.UtcNow
                        }, stoppingToken);

                        _lastStatisticsUpdate = DateTime.UtcNow;
                    }

                    // Broadcast recent alerts
                    var recentAlerts = _dataService.GetRecentAlerts(5);
                    if (recentAlerts.Any())
                    {
                        await _hubContext.Clients.All.SendAsync("NewAlerts", recentAlerts, stoppingToken);
                    }

                    await Task.Delay(1000, stoppingToken); // 1000ms interval (1 second)
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in real-time broadcast");
                    await Task.Delay(1000, stoppingToken); // Wait before retrying
                }
            }
        }

        private async Task StartDataPurgeTask(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Purge old data every hour
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                    _dataService.PurgeOldData();
                    _logger.LogInformation("Data purge completed");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during data purge");
                }
            }
        }
    }
}
