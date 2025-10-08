# 🚀 Quick Deployment Instructions

## Step 1: Deploy Backend to Railway (5 minutes)

### 1.1 Go to Railway
1. Visit [railway.app](https://railway.app)
2. **Sign in with GitHub**
3. Click **"New Project"**

### 1.2 Connect Repository
1. Select **"Deploy from GitHub repo"**
2. Choose **`vinil2001/RealTimeAnalyticsDashboard`**
3. Railway will automatically detect the .NET project

### 1.3 Add PostgreSQL Database
1. In your project dashboard, click **"New"**
2. Select **"Database"** → **"PostgreSQL"**
3. Railway automatically creates database and sets `DATABASE_URL`

### 1.4 Configure Environment Variables
Click **"Variables"** tab and add:
```
ASPNETCORE_ENVIRONMENT=Production
FRONTEND_URL=https://your-app-name.netlify.app
```
(Replace `your-app-name` with your actual Netlify subdomain)

### 1.5 Deploy
1. Railway automatically builds and deploys
2. **Copy your Railway URL**: `https://your-project.up.railway.app`
3. Test health endpoint: `https://your-project.up.railway.app/health`

---

## Step 2: Deploy Frontend to Netlify (3 minutes)

### 2.1 Update Environment
1. **Edit** `sensor-dashboard/src/environments/environment.prod.ts`
2. **Replace** the URLs with your actual Railway backend URL:
```typescript
export const environment = {
  production: true,
  apiUrl: 'https://YOUR-RAILWAY-URL.up.railway.app/api',
  signalRUrl: 'https://YOUR-RAILWAY-URL.up.railway.app/sensorHub'
};
```

### 2.2 Commit Changes
```bash
git add .
git commit -m "Update production environment URLs"
git push origin main
```

### 2.3 Deploy to Netlify
1. Visit [netlify.com](https://netlify.com)
2. **Sign in with GitHub**
3. Click **"New site from Git"**
4. Select **GitHub** → **`vinil2001/RealTimeAnalyticsDashboard`**
5. **Build settings**:
   - Base directory: `sensor-dashboard`
   - Build command: `npm run build --configuration=production`
   - Publish directory: `sensor-dashboard/dist/sensor-dashboard`

### 2.4 Configure Netlify
1. **Site settings** → **Environment variables**
2. Add: `NODE_VERSION = 18`
3. **Deploy** automatically starts

---

## Step 3: Update CORS Configuration (1 minute)

### 3.1 Get Netlify URL
1. After Netlify deployment, copy your URL: `https://your-app-name.netlify.app`

### 3.2 Update Railway Environment
1. Go back to **Railway dashboard**
2. **Variables** → Update `FRONTEND_URL`:
```
FRONTEND_URL=https://your-app-name.netlify.app
```
3. Railway automatically redeploys with new CORS settings

---

## Step 4: Test Deployment ✅

### 4.1 Backend Test
Visit: `https://your-railway-url.up.railway.app/health`

Expected response:
```json
{
  "status": "healthy",
  "timestamp": "2025-01-08T11:00:00Z",
  "environment": "Production",
  "database": "PostgreSQL"
}
```

### 4.2 Frontend Test
Visit: `https://your-netlify-url.netlify.app`

Expected behavior:
- ✅ Dashboard loads
- ✅ Status shows "Connected"
- ✅ Real-time data appears
- ✅ Charts update automatically
- ✅ Filters work correctly

---

## 🎉 Deployment Complete!

Your Real-Time Analytics Dashboard is now live:
- **Frontend**: `https://your-app-name.netlify.app`
- **Backend**: `https://your-project.up.railway.app`
- **Database**: PostgreSQL on Railway
- **Cost**: $0 (free tiers)

---

## 🔧 Troubleshooting

### Common Issues:

1. **CORS Errors**: 
   - Check `FRONTEND_URL` in Railway variables
   - Ensure it matches your Netlify URL exactly

2. **Build Failures**:
   - Check Node.js version (should be 18)
   - Verify all dependencies are in package.json

3. **Database Connection**:
   - Railway automatically sets `DATABASE_URL`
   - Check Railway logs for connection errors

4. **SignalR Not Working**:
   - Ensure WebSocket support is enabled
   - Check browser console for connection errors

### Getting Help:
- **Railway**: Check deployment logs in dashboard
- **Netlify**: Check build logs in site dashboard
- **GitHub**: Create issue in repository
