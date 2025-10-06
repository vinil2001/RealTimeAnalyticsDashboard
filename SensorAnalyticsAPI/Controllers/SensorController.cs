using Microsoft.AspNetCore.Mvc;
using SensorAnalyticsAPI.Models;
using SensorAnalyticsAPI.Services;

namespace SensorAnalyticsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorController : ControllerBase
    {
        private readonly ISensorDataService _dataService;
        private readonly ILogger<SensorController> _logger;

        public SensorController(ISensorDataService dataService, ILogger<SensorController> logger)
        {
            _dataService = dataService;
            _logger = logger;
        }

        [HttpGet("readings")]
        public ActionResult<IEnumerable<SensorReading>> GetRecentReadings([FromQuery] int count = 1000)
        {
            try
            {
                var readings = _dataService.GetRecentReadings(count);
                return Ok(readings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent readings");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("readings/{sensorId}")]
        public ActionResult<IEnumerable<SensorReading>> GetSensorReadings(string sensorId, [FromQuery] int count = 100)
        {
            try
            {
                var readings = _dataService.GetReadingsBySensor(sensorId, count);
                return Ok(readings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving readings for sensor {sensorId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("statistics")]
        public ActionResult<IEnumerable<SensorStatistics>> GetAllStatistics()
        {
            try
            {
                var statistics = _dataService.GetAllSensorStatistics();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving statistics");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("statistics/{sensorId}")]
        public ActionResult<SensorStatistics> GetSensorStatistics(string sensorId)
        {
            try
            {
                var statistics = _dataService.GetSensorStatistics(sensorId);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving statistics for sensor {sensorId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("alerts")]
        public ActionResult<IEnumerable<AnomalyAlert>> GetRecentAlerts([FromQuery] int count = 50)
        {
            try
            {
                var alerts = _dataService.GetRecentAlerts(count);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving alerts");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("health")]
        public ActionResult<object> GetHealth()
        {
            try
            {
                var totalReadings = _dataService.GetTotalReadingsCount();
                var statistics = _dataService.GetAllSensorStatistics();
                var recentAlerts = _dataService.GetRecentAlerts(5);

                return Ok(new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    totalReadings = totalReadings,
                    activeSensors = statistics.Count(),
                    recentAlerts = recentAlerts.Count(),
                    memoryUsage = GC.GetTotalMemory(false)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving health status");
                return StatusCode(500, new { status = "unhealthy", error = ex.Message });
            }
        }

        [HttpPost("readings")]
        public ActionResult AddReading([FromBody] SensorReading reading)
        {
            try
            {
                if (reading == null)
                {
                    return BadRequest("Reading cannot be null");
                }

                if (string.IsNullOrEmpty(reading.SensorId))
                {
                    return BadRequest("SensorId is required");
                }

                _dataService.AddReading(reading);
                return Ok(new { message = "Reading added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding reading");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("purge")]
        public ActionResult PurgeOldData()
        {
            try
            {
                _dataService.PurgeOldData();
                return Ok(new { message = "Data purged successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error purging data");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
