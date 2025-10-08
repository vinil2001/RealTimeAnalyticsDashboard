# Port Configuration Guide

## 🔧 Configurable Ports

Both frontend and backend now support configurable ports for easy development and deployment.

## Backend Configuration

### 1. **appsettings.json** (Main Configuration)
```json
{
  "ServerSettings": {
    "HttpPort": 5000,
    "HttpsPort": 7206,
    "FrontendUrl": "http://localhost:4200"
  }
}
```

### 2. **Environment Variables** (Override Configuration)
```bash
# For development
PORT=5000
FRONTEND_URL=http://localhost:4200

# For production (Railway)
PORT=8080
FRONTEND_URL=https://your-app.netlify.app
```

### 3. **Launch Profiles** (Development Only)
Update `Properties/launchSettings.json`:
```json
{
  "profiles": {
    "http": {
      "applicationUrl": "http://localhost:5000"
    },
    "https": {
      "applicationUrl": "https://localhost:7206;http://localhost:5000"
    }
  }
}
```

## Frontend Configuration

### 1. **App Configuration** (`src/app/config/app.config.ts`)
```typescript
export const APP_CONFIG: AppConfig = {
  backend: {
    httpPort: 5000,     // Backend HTTP port
    httpsPort: 7206,    // Backend HTTPS port
    host: 'localhost',  // Backend host
    useHttps: false     // Use HTTPS in production
  },
  frontend: {
    port: 4200          // Angular dev server port
  }
};
```

### 2. **Angular CLI Configuration**
```bash
# Start on custom port
ng serve --port 4200

# Or configure in angular.json
"serve": {
  "builder": "@angular-devkit/build-angular:dev-server",
  "options": {
    "port": 4200
  }
}
```

## 🚀 Quick Port Changes

### Change Backend Port
1. **Update** `appsettings.json`:
   ```json
   "ServerSettings": {
     "HttpPort": 5001  // New port
   }
   ```
2. **Restart backend**: `dotnet run`

### Change Frontend Port
1. **Update** `app.config.ts`:
   ```typescript
   backend: {
     httpPort: 5001  // Match backend port
   }
   ```
2. **Restart frontend**: `npm start`

## 🔄 Common Port Configurations

### Configuration 1: Default (Current)
- **Backend**: `http://localhost:5000`
- **Frontend**: `http://localhost:4200`

### Configuration 2: Alternative Ports
- **Backend**: `http://localhost:5001`
- **Frontend**: `http://localhost:4201`

### Configuration 3: Production-like
- **Backend**: `http://localhost:8080`
- **Frontend**: `http://localhost:3000`

## 🛠️ Environment-Specific Settings

### Development
```json
// appsettings.Development.json
{
  "ServerSettings": {
    "HttpPort": 5000,
    "FrontendUrl": "http://localhost:4200"
  }
}
```

### Production
```bash
# Environment variables
PORT=8080
FRONTEND_URL=https://your-frontend-domain.com
```

## 🔍 Troubleshooting

### Port Already in Use
```bash
# Find process using port
netstat -ano | findstr :5000

# Kill process
taskkill /F /PID <process_id>
```

### CORS Issues
Make sure `FrontendUrl` in backend configuration matches your actual frontend URL.

### SignalR Connection Issues
Verify that both `apiUrl` and `signalRUrl` in frontend point to the same backend port.

## 📋 Quick Commands

```bash
# Backend with custom port
dotnet run --urls="http://localhost:5001"

# Frontend with custom port
ng serve --port 4201

# Check what's running on ports
netstat -ano | findstr :5000
netstat -ano | findstr :4200
```
