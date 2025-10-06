# Quick Fix for Connection Issue

## Problem
Frontend shows "Disconnected" and cannot connect to backend.

## Solution
The frontend is trying to connect to the wrong port. Follow these steps:

### Step 1: Restart Angular with New Configuration
1. Stop Angular (Ctrl+C in the terminal)
2. Restart Angular:
```bash
cd D:\Development\RealTimeAnalyticsDashboard\sensor-dashboard
npm start
```

### Step 2: Verify Backend Port
Check that backend is running on correct port:
```bash
cd D:\Development\RealTimeAnalyticsDashboard\SensorAnalyticsAPI
dotnet run
```

You should see:
```
Now listening on: https://localhost:7206
Now listening on: http://localhost:5089
```

### Step 3: Test Connection
Open in browser: `https://localhost:7206/health`

If you get SSL/certificate error, use HTTP instead:
1. Edit `src/environments/environment.ts`
2. Change to:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5089/api',
  signalRUrl: 'http://localhost:5089/sensorHub'
};
```
3. Restart Angular

### Expected Result
After restart, you should see:
- ✅ Status changes from "Disconnected" to "Connected"
- ✅ Metrics show real data (Total Readings > 0)
- ✅ Live charts with sensor data
- ✅ Alerts appearing in the alerts panel

## If Still Not Working
See `TROUBLESHOOTING.md` for detailed solutions.
