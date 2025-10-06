# Speed Configuration Options

## Current Settings (Optimized for Visualization)

### Data Generation Speed
- **100 readings/second** (reduced from 1000)
- **2 readings per sensor per second** (reduced from 20)
- **500ms interval** per sensor reading

### UI Update Speed  
- **1 second intervals** for chart updates (reduced from 100ms)
- **10 seconds** for statistics updates (reduced from 5s)
- **20 readings per update** (reduced from 50)

## Alternative Configurations

### 1. Slow Mode (Best for Demos)
```csharp
// In SensorSimulationService.cs
var readingsPerSensorPerSecond = 1;  // 1 reading per sensor per second
var intervalMs = 1000;               // 1 second interval

// In RealTimeUpdateService.cs  
await Task.Delay(2000, stoppingToken); // 2 second UI updates
```

### 2. Medium Mode (Current)
```csharp
// In SensorSimulationService.cs
var readingsPerSensorPerSecond = 2;  // 2 readings per sensor per second
var intervalMs = 500;                // 500ms interval

// In RealTimeUpdateService.cs
await Task.Delay(1000, stoppingToken); // 1 second UI updates
```

### 3. Fast Mode (Original High Performance)
```csharp
// In SensorSimulationService.cs
var readingsPerSensorPerSecond = 20; // 20 readings per sensor per second
var intervalMs = 50;                 // 50ms interval

// In RealTimeUpdateService.cs
await Task.Delay(100, stoppingToken); // 100ms UI updates
```

### 4. Ultra-Slow Mode (For Detailed Analysis)
```csharp
// In SensorSimulationService.cs
var readingsPerSensorPerSecond = 0.5; // 1 reading per 2 seconds per sensor
var intervalMs = 2000;                // 2 second interval

// In RealTimeUpdateService.cs
await Task.Delay(5000, stoppingToken); // 5 second UI updates
```

## How to Change Speed

1. **Stop the backend** (Ctrl+C)
2. **Edit the values** in the service files
3. **Restart the backend**: `dotnet run`
4. **Refresh the frontend** to see changes

## Current Benefits

- ✅ **Smoother visualization** - easier to follow chart changes
- ✅ **Better user experience** - not overwhelming with data
- ✅ **Clearer trends** - easier to spot patterns and anomalies
- ✅ **Reduced CPU usage** - more efficient processing
- ✅ **Better for demos** - clients can actually see what's happening
