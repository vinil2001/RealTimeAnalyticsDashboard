using Microsoft.AspNetCore.SignalR;
using SensorAnalyticsAPI.Services;

namespace SensorAnalyticsAPI.Hubs
{
    public class SensorDataHub : Hub
    {
        private readonly ISensorDataService _dataService;
        private readonly ILogger<SensorDataHub> _logger;

        public SensorDataHub(ISensorDataService dataService, ILogger<SensorDataHub> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation($"Client {Context.ConnectionId} joined group {groupName}");
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation($"Client {Context.ConnectionId} left group {groupName}");
        }

        public async Task GetInitialData()
        {
            try
            {
                var recentReadings = _dataService.GetRecentReadings(100);
                var statistics = _dataService.GetAllSensorStatistics();
                var alerts = _dataService.GetRecentAlerts(20);

                await Clients.Caller.SendAsync("InitialData", new
                {
                    readings = recentReadings,
                    statistics = statistics,
                    alerts = alerts,
                    totalCount = _dataService.GetTotalReadingsCount()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending initial data to client");
                await Clients.Caller.SendAsync("Error", "Failed to load initial data");
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
