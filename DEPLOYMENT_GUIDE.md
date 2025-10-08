# Deployment Guide - Railway + Netlify

## 🚀 Step 1: Deploy Backend to Railway

### 1.1 Create Railway Account
1. Go to [railway.app](https://railway.app)
2. Sign up with GitHub account
3. Connect your GitHub repository

### 1.2 Deploy Backend
1. **Create New Project** on Railway
2. **Connect GitHub Repository**: `vinil2001/RealTimeAnalyticsDashboard`
3. **Add PostgreSQL Database**:
   - Click "New" → "Database" → "PostgreSQL"
   - Railway will automatically create database and connection string

### 1.3 Configure Environment Variables
In Railway dashboard, go to your project → Variables:
```bash
# Automatically set by Railway PostgreSQL:
DATABASE_URL=postgresql://username:password@host:port/database

# Set these manually:
ASPNETCORE_ENVIRONMENT=Production
FRONTEND_URL=https://your-app-name.netlify.app
PORT=8080
```

### 1.4 Deploy Configuration
Railway will automatically:
- ✅ Detect .NET project
- ✅ Build using `railway.toml` configuration
- ✅ Set up PostgreSQL connection
- ✅ Deploy to custom domain

**Expected Result**: Backend running at `https://your-project-name.up.railway.app`

---

## 🌐 Step 2: Deploy Frontend to Netlify

### 2.1 Prepare Frontend for Production

First, update the Angular environment for production:

```typescript
// src/environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://your-backend-url.up.railway.app/api',
  signalRUrl: 'https://your-backend-url.up.railway.app/sensorHub'
};
```

### 2.2 Build Production Frontend
```bash
cd sensor-dashboard
npm install
npm run build --prod
```

### 2.3 Deploy to Netlify
1. **Go to [netlify.com](https://netlify.com)**
2. **Sign up with GitHub**
3. **New Site from Git** → Connect GitHub
4. **Select Repository**: `vinil2001/RealTimeAnalyticsDashboard`
5. **Build Settings**:
   - Base directory: `sensor-dashboard`
   - Build command: `npm run build`
   - Publish directory: `sensor-dashboard/dist/sensor-dashboard`

### 2.4 Configure Netlify
**Site Settings** → **Environment Variables**:
```bash
NODE_VERSION=18
```

**Expected Result**: Frontend running at `https://your-app-name.netlify.app`

---

## 🔧 Step 3: Connect Frontend to Backend

### 3.1 Update Backend CORS
The backend is already configured to accept the frontend URL via `FRONTEND_URL` environment variable.

### 3.2 Update Frontend Environment
Replace `your-backend-url.up.railway.app` with your actual Railway backend URL.

---

## 📋 Step 4: Verify Deployment

### 4.1 Backend Health Check
Visit: `https://your-backend-url.up.railway.app/health`

Expected response:
```json
{
  "status": "healthy",
  "timestamp": "2025-01-07T10:00:00Z",
  "environment": "Production",
  "database": "PostgreSQL"
}
```

### 4.2 Frontend Functionality
1. ✅ Dashboard loads successfully
2. ✅ Connection status shows "Connected"
3. ✅ Real-time data appears in charts
4. ✅ Sensor filters work correctly
5. ✅ Statistics update automatically
6. ✅ Alerts appear in real-time

---

## 🛠️ Troubleshooting

### Backend Issues
```bash
# Check Railway logs
railway logs

# Check database connection
railway connect postgresql
```

### Frontend Issues
```bash
# Check Netlify build logs in dashboard
# Verify environment variables
# Check browser console for CORS errors
```

### Common Problems

1. **CORS Errors**: Update `FRONTEND_URL` in Railway
2. **Database Connection**: Check `DATABASE_URL` format
3. **SignalR Issues**: Ensure WebSocket support enabled
4. **Build Failures**: Check Node.js version compatibility

---

## 💰 Cost Breakdown

### Railway (Backend + Database)
- ✅ **Free Tier**: $5/month credit
- ✅ **PostgreSQL**: Included in free tier
- ✅ **Custom Domain**: Free
- ✅ **SSL Certificate**: Automatic

### Netlify (Frontend)
- ✅ **Free Tier**: 100GB bandwidth/month
- ✅ **Custom Domain**: Free
- ✅ **SSL Certificate**: Automatic
- ✅ **GitHub Integration**: Free

**Total Monthly Cost**: $0 (within free tiers)

---

## 🚀 Next Steps After Deployment

1. **Custom Domain**: Add your own domain to both services
2. **Monitoring**: Set up uptime monitoring
3. **Analytics**: Add Google Analytics to frontend
4. **Scaling**: Monitor usage and upgrade plans if needed
5. **Backup**: Set up database backups on Railway

---

## 📞 Support

- **Railway**: [docs.railway.app](https://docs.railway.app)
- **Netlify**: [docs.netlify.com](https://docs.netlify.com)
- **Project Issues**: GitHub repository issues
