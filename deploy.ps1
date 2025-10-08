# Deployment Script for Real-Time Analytics Dashboard

Write-Host "🚀 Starting deployment process..." -ForegroundColor Green

# Step 1: Build Frontend
Write-Host "📦 Building Angular frontend..." -ForegroundColor Yellow
Set-Location "sensor-dashboard"
npm install
npm run build --configuration=production

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Frontend build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Frontend build successful!" -ForegroundColor Green

# Step 2: Test Backend Build
Write-Host "🔧 Testing backend build..." -ForegroundColor Yellow
Set-Location "..\SensorAnalyticsAPI"
dotnet build --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Backend build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Backend build successful!" -ForegroundColor Green

# Step 3: Commit and Push Changes
Write-Host "📤 Pushing changes to GitHub..." -ForegroundColor Yellow
Set-Location ".."
git add .
git commit -m "Prepare for production deployment - Frontend build configuration"
git push origin main

Write-Host "✅ Changes pushed to GitHub!" -ForegroundColor Green

Write-Host "🎉 Deployment preparation complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Deploy backend to Railway: https://railway.app" -ForegroundColor White
Write-Host "2. Deploy frontend to Netlify: https://netlify.com" -ForegroundColor White
Write-Host "3. Update environment URLs after deployment" -ForegroundColor White
Write-Host ""
Write-Host "📖 See DEPLOYMENT_GUIDE.md for detailed instructions" -ForegroundColor Cyan
