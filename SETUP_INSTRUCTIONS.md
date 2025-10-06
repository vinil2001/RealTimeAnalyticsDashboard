# Setup Instructions - New Location

## 📍 Project Location
The project has been moved to: **D:\Development\RealTimeAnalyticsDashboard**

## 🚀 Quick Start

### 1. Open Project Directory
```bash
cd D:\Development\RealTimeAnalyticsDashboard
```

### 2. Start Backend API
```bash
cd SensorAnalyticsAPI
dotnet run
```
✅ API will be available at: `https://localhost:7206` (HTTPS) or `http://localhost:5089` (HTTP)

### 3. Start Frontend
```bash
cd ..\sensor-dashboard
npm start
```
✅ Dashboard will be available at: `http://localhost:4200`

## 🔧 Development Setup

### Backend (.NET API)
```bash
cd D:\Development\RealTimeAnalyticsDashboard\SensorAnalyticsAPI
dotnet restore
dotnet build
dotnet run
```

### Frontend (Angular)
```bash
cd D:\Development\RealTimeAnalyticsDashboard\sensor-dashboard
npm install
npm start
```

## 📊 Performance Testing
```bash
cd D:\Development\RealTimeAnalyticsDashboard
node performance-test.js
```

## 📁 Project Structure
```
D:\Development\RealTimeAnalyticsDashboard\
├── SensorAnalyticsAPI/          # .NET Core backend
├── sensor-dashboard/            # Angular frontend
├── performance-test.js          # Performance testing script
├── README.md                    # Main documentation
├── ARCHITECTURE_DECISIONS.md    # Architecture decisions
├── PERFORMANCE_REPORT.md        # Performance analysis
├── PROJECT_SUMMARY.md           # Project completion summary
└── TROUBLESHOOTING.md           # Connection issues guide
```

## ✅ Verification Steps

1. **Backend API**: Navigate to `https://localhost:7206/health` or `http://localhost:5089/health`
2. **API Documentation**: Visit `https://localhost:7206/swagger` (if available)
3. **Real-time Data**: Check `https://localhost:7206/api/sensor/readings`
4. **Performance**: Run `node performance-test.js`

## 🎯 Current Status
- ✅ **Backend**: Fully functional and running
- ✅ **Data Simulation**: 1000+ readings/second
- ✅ **API Endpoints**: All working correctly
- ✅ **Documentation**: Complete and up-to-date
- ✅ **Frontend**: Ready and configured for correct ports

## 🔧 Connection Issues?
If frontend cannot connect to backend, see `TROUBLESHOOTING.md` for detailed solutions.

The project is successfully relocated and fully operational in the new location!
