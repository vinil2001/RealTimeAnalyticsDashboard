# Real-Time Sensor Analytics Dashboard

A high-performance real-time analytics dashboard that processes and displays streaming sensor data with anomaly detection and alerting capabilities.

## 🚀 Features

- **Real-time Data Processing**: Handles 1000+ sensor readings per second
- **Live Dashboard**: Angular-based dashboard with real-time charts and statistics
- **Anomaly Detection**: Statistical outlier detection with configurable thresholds
- **Alert System**: Real-time notifications for sensor anomalies
- **Performance Optimized**: Maintains 100,000 data points in memory efficiently
- **Auto-purging**: 24-hour data retention with automatic cleanup

## 🏗️ Architecture

### Backend (.NET Core 9.0)
- **API**: RESTful endpoints for data access
- **Real-time Communication**: SignalR for live updates
- **Data Storage**: High-performance in-memory storage
- **Simulation**: 50 virtual sensors generating realistic data
- **Anomaly Detection**: 3-sigma statistical analysis

### Frontend (Angular 18)
- **Dashboard**: Responsive real-time data visualization
- **Charts**: Chart.js with real-time data streaming
- **Alerts**: Live notification system
- **Statistics**: Comprehensive sensor analytics
- **Filtering**: Sensor type and data filtering capabilities

## 📋 Requirements

- **Backend**: .NET 9.0 SDK
- **Frontend**: Node.js 18+ and npm
- **Development**: Visual Studio Code or Visual Studio 2022

## 🛠️ Installation & Setup

### 1. Navigate to Project Directory
```bash
cd D:\Development\RealTimeAnalyticsDashboard
```

### 2. Backend Setup (.NET API)
```bash
cd SensorAnalyticsAPI
dotnet restore
dotnet build
```

### 3. Frontend Setup (Angular)
```bash
cd ../sensor-dashboard
npm install
```

## 🚀 Running the Application

### Start Backend API
```bash
cd SensorAnalyticsAPI
dotnet run
```
The API will be available at: `https://localhost:7000`

### Start Frontend Dashboard
```bash
cd sensor-dashboard
npm start
```
The dashboard will be available at: `http://localhost:4200`

## 📊 API Endpoints

### Health & Status
- `GET /health` - System health check
- `GET /api/sensor/health` - Detailed health metrics

### Sensor Data
- `GET /api/sensor/readings?count=1000` - Recent sensor readings
- `GET /api/sensor/readings/{sensorId}?count=100` - Sensor-specific readings
- `POST /api/sensor/readings` - Add new reading (for testing)

### Statistics
- `GET /api/sensor/statistics` - All sensor statistics
- `GET /api/sensor/statistics/{sensorId}` - Specific sensor statistics

### Alerts
- `GET /api/sensor/alerts?count=50` - Recent anomaly alerts

### Real-time
- **SignalR Hub**: `/sensorHub` - Real-time data streaming

## 🔧 Configuration

### Backend Configuration (appsettings.json)
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "SensorAnalyticsAPI": "Debug"
    }
  },
  "AllowedHosts": "*"
}
```

### Frontend Configuration (environment.ts)
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7000/api',
  signalRUrl: 'https://localhost:7000/sensorHub'
};
```

## 📈 Performance Testing

### Run Performance Tests
```bash
node performance-test.js
```

### Expected Performance Metrics
- **Throughput**: 1000+ readings/second
- **Response Time**: <50ms average
- **Memory Usage**: ~15MB for 100K readings
- **Concurrent Users**: 100+ simultaneous connections

## 🏭 Sensor Simulation

The system simulates 50 sensors across different types:
- **Temperature Sensors** (10): -10°C to 50°C
- **Humidity Sensors** (10): 0% to 100%
- **Pressure Sensors** (10): 950-1050 hPa
- **Light Sensors** (10): 0-2000 lux
- **Air Quality Sensors** (10): 0-500 AQI

Each sensor generates 20 readings per second with realistic drift and noise patterns.

## 🚨 Anomaly Detection

### Algorithm
- **Method**: 3-sigma statistical analysis
- **Threshold**: 3 standard deviations from mean
- **Sensitivity**: Configurable severity levels
- **Real-time**: Immediate alert generation

### Alert Severity Levels
- **Low**: 1-2 sigma deviation
- **Medium**: 2-3 sigma deviation  
- **High**: 3-4 sigma deviation
- **Critical**: >4 sigma deviation

## 📱 Dashboard Features

### Real-time Charts
- Multi-sensor data visualization
- Configurable time windows
- Interactive filtering
- Performance-optimized rendering

### Statistics Panel
- Live sensor statistics
- Health status indicators
- Performance metrics
- Data quality indicators

### Alert Management
- Real-time alert notifications
- Severity-based color coding
- Alert history and filtering
- Detailed alert information

## 🔍 Monitoring & Observability

### Health Checks
- API endpoint health
- Memory usage monitoring
- Data processing rates
- Connection status

### Performance Metrics
- Requests per second
- Response times
- Memory consumption
- Error rates

## 🚀 Production Deployment

### Recommended Enhancements
1. **Database Integration**: PostgreSQL/SQL Server for persistence
2. **Caching**: Redis for distributed caching
3. **Load Balancing**: Multiple API instances
4. **Monitoring**: Application Insights/Prometheus
5. **Security**: JWT authentication, API keys
6. **Containerization**: Docker deployment

### Docker Deployment (Planned)
```dockerfile
# Backend
FROM mcr.microsoft.com/dotnet/aspnet:9.0
COPY . /app
WORKDIR /app
EXPOSE 80
ENTRYPOINT ["dotnet", "SensorAnalyticsAPI.dll"]

# Frontend  
FROM nginx:alpine
COPY dist/sensor-dashboard /usr/share/nginx/html
EXPOSE 80
```

## 🧪 Testing

### Unit Tests
```bash
cd SensorAnalyticsAPI
dotnet test
```

### Integration Tests
```bash
cd SensorAnalyticsAPI
dotnet test --filter Category=Integration
```

### Frontend Tests
```bash
cd sensor-dashboard
npm test
```

### Performance Tests
```bash
node performance-test.js
```

## 📚 Documentation

- **Architecture Decisions**: `ARCHITECTURE_DECISIONS.md`
- **Performance Report**: `PERFORMANCE_REPORT.md`
- **API Documentation**: Available at `/swagger` when running
- **Code Documentation**: Inline XML documentation

## 🤝 AI Tool Usage

This project demonstrates effective AI tool integration:

### AI-Generated Components
- Service layer implementations
- Data models and interfaces
- Configuration files
- Performance testing scripts
- Documentation templates

### Human-Optimized Areas
- Performance tuning and optimization
- Real-world testing and validation
- Architecture decisions
- Production deployment strategies

## 🐛 Troubleshooting

### Common Issues

1. **CORS Errors**
   - Ensure backend is running on https://localhost:7000
   - Check CORS configuration in Program.cs

2. **SignalR Connection Issues**
   - Verify SSL certificates for localhost
   - Check browser console for connection errors

3. **Performance Issues**
   - Monitor memory usage
   - Check for memory leaks
   - Verify data purging is working

4. **Build Errors**
   - Ensure .NET 9.0 SDK is installed
   - Run `dotnet restore` and `npm install`

### Support
For issues and questions, please check the documentation files or create an issue in the repository.

## 📄 License

This project is a proof-of-concept demonstration and is provided as-is for evaluation purposes.

---

**Built with ❤️ using .NET Core, Angular, and AI assistance**
