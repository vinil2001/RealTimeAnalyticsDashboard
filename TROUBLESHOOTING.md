# Troubleshooting Guide

## Frontend Cannot Connect to Backend

### Problem
```
Error starting SignalR connection: FailedToNegotiateWithServerError
GET https://localhost:7000/api/sensor/readings net::ERR_CONNECTION_REFUSED
```

### Solution

#### 1. Check Ports
Backend is running on:
- **HTTPS**: `https://localhost:7206`
- **HTTP**: `http://localhost:5089`

Frontend is configured for: `https://localhost:7206`

#### 2. Verify Backend is Running
```bash
cd D:\Development\RealTimeAnalyticsDashboard\SensorAnalyticsAPI
dotnet run
```

You should see:
```
info: Microsoft.Hosting.Lifetime[0]
      Now listening on: https://localhost:7206
      Now listening on: http://localhost:5089
```

#### 3. Test API Endpoints
Open in browser:
- `https://localhost:7206/health` (HTTPS)
- `http://localhost:5089/health` (HTTP)

#### 4. If HTTPS Doesn't Work

**Option A: Use HTTP**
1. Replace `src/environments/environment.ts` content:
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5089/api',
  signalRUrl: 'http://localhost:5089/sensorHub'
};
```

**Option B: Configure HTTPS Certificate**
```bash
dotnet dev-certs https --trust
```

#### 5. Restart Services
1. Stop Angular (Ctrl+C)
2. Stop .NET API (Ctrl+C)
3. Start .NET API:
   ```bash
   cd D:\Development\RealTimeAnalyticsDashboard\SensorAnalyticsAPI
   dotnet run
   ```
4. Start Angular:
   ```bash
   cd D:\Development\RealTimeAnalyticsDashboard\sensor-dashboard
   npm start
   ```

#### 6. Verify CORS Configuration
Ensure `Program.cs` contains:
```csharp
policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
```

### Expected Results After Fix:
- Connection status: "Connected" (green)
- Metrics show real data
- Charts display live data
- Alerts appear in panel

### Additional Checks

#### Network Connection Verification:
```bash
# Check if ports are listening
netstat -an | findstr :7206
netstat -an | findstr :5089
```

#### Browser Verification:
1. Open Developer Tools (F12)
2. Go to Network tab
3. Refresh page
4. Check API request statuses

#### If Still Not Working:
1. Check Windows Firewall
2. Check antivirus software
3. Try running as administrator
4. Verify ports aren't occupied by other programs

### Successful Connection
When everything works correctly, you'll see:
- ✅ "Connected" status
- ✅ Data in metrics (Total Readings > 0)
- ✅ Live charts with data
- ✅ Anomaly alerts
- ✅ Readings/Sec > 0
