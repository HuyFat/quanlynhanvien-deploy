# Deploy to Railway - Step by Step Guide

## Prerequisites
- GitHub account
- Railway account (free at https://railway.app)
- Supabase database connection string (already have from setup)

## Step 1: Push code to GitHub

### 1.1 Initialize git repository
```powershell
cd c:\Users\Gia Huy\source\repos\thuctap
git init
git config user.name "Your Name"
git config user.email "your.email@example.com"
```

### 1.2 Create .gitignore and README (Already done)
Files are already created:
- `.gitignore` - Prevents committing unnecessary files
- `README.md` - Documentation

### 1.3 Commit all files
```powershell
git add .
git commit -m "Initial commit: Employee management system with Supabase"
```

### 1.4 Create GitHub repository
1. Go to https://github.com/new
2. Fill in:
   - Repository name: `quanlynhanvien-deploy` (or your preferred name)
   - Description: `Employee Management System - ASP.NET Core 9.0`
   - Visibility: Public (required for Railway free tier)
3. Click "Create repository"

### 1.5 Push to GitHub
From the GitHub repo page, copy the HTTPS URL and run:
```powershell
git remote add origin https://github.com/YOUR_USERNAME/quanlynhanvien-deploy.git
git branch -M main
git push -u origin main
```

## Step 2: Deploy on Railway

### 2.1 Sign up to Railway
1. Go to https://railway.app
2. Click "Start Free" or "Login"
3. Sign up with GitHub (easiest method)
4. Authorize Railway to access your GitHub account

### 2.2 Create new Railway project
1. After login, click "Create New Project"
2. Select "Deploy from GitHub repo"
3. Find and select `quanlynhanvien-deploy` repository
4. Click "Deploy"
5. Wait for Railway to detect the project (it will auto-detect .NET)

### 2.3 Wait for build
- Railway will automatically:
  - Clone your repository
  - Detect it's a .NET project
  - Run `dotnet publish -c Release`
  - Start the application

This typically takes 2-5 minutes. You can monitor progress in the build logs.

### 2.4 Check deployment status
- Go to your Railway project dashboard
- View logs to ensure no errors
- Once complete, you'll see a green "Running" status

## Step 3: Configure Environment Variables

### 3.1 Set database connection string
1. In Railway project, go to **Variables** tab
2. Add new variable:
   - **Key**: `ConnectionStrings__DefaultConnection`
   - **Value**: Your Supabase connection string
   
Example:
```
Host=db.kqddliovbvabfnhzbjgd.supabase.co;Database=postgres;Username=postgres;Password=Nguyentruonggiahuy2k4;Port=5432;SSL Mode=Require;Trust Server Certificate=true
```

3. Click "Save"

### 3.2 Configure other settings (optional)
```
ASPNETCORE_ENVIRONMENT=Production
```

## Step 4: Get your public URL

### 4.1 Find the deployment URL
1. In Railway dashboard, look for **Deployments** tab
2. You'll see a URL like: `https://quanlynhanvien-deploy-production.up.railway.app`
3. Click the URL or copy it

### 4.2 Test the application
- Open the URL in browser on your desktop
- Should see your application homepage
- Try to add/edit/delete employees to verify database connection works

## Step 5: Access from Mobile

### 5.1 Mobile on same network or different network
- On any phone/tablet with internet:
  - Open browser
  - Type: `https://quanlynhanvien-deploy-production.up.railway.app`
  - You can now manage employees from any device!

### 5.2 Troubleshooting mobile access
- If page doesn't load, check:
  - Internet connection is active
  - Domain URL is correct (copy from Railway dashboard)
  - Wait 1-2 minutes after deployment for DNS to propagate

## Common Issues & Solutions

### Issue: Build failed with errors
**Solution:**
- Check Railway build logs for specific error
- Common issues:
  - Wrong .NET version: Update `Program.cs` and `.csproj`
  - Missing NuGet packages: Run `dotnet restore` locally and push again
  - Connection string syntax error: Verify in `appsettings.json`

### Issue: Application crashes after deployment
**Solution:**
- Check Railway application logs
- Verify `ConnectionStrings__DefaultConnection` environment variable is set correctly
- Ensure Supabase database is accessible from Railway (check firewall rules)

### Issue: Cannot connect to database
**Solution:**
- Verify connection string is correct:
  - Host: Should be `db.xxxxx.supabase.co`
  - Port: Should be `5432`
  - Username: `postgres`
  - SSL Mode: Must be `Require`
- Test locally: Run `dotnet run` with the same connection string
- Check Supabase: Go to Supabase dashboard > Settings > Database > verify credentials

### Issue: 502 Bad Gateway error
**Solution:**
- Application may still be starting (wait 1-2 minutes)
- Check application logs in Railway
- Restart deployment: In Railway project, click "Redeploy"

## Monitoring & Maintenance

### Monitor application logs
- Railway > Logs tab shows real-time application logs
- Monitor for any errors or exceptions

### Scale/Upgrade (if needed)
- Railway > Settings
- Increase memory/CPU resources if needed
- By default, free tier has sufficient resources for this application

### Update code
To deploy new changes:
```powershell
git add .
git commit -m "Update: description of changes"
git push origin main
```

Railway automatically redeploys when you push to main branch!

## Summary

✅ **You now have:**
- Code hosted on GitHub
- Application deployed on Railway (always running)
- Database on Supabase (accessible worldwide)
- Public URL accessible from any device
- Mobile access working from any network

**Your application is live and ready to use from anywhere!**

## Next Steps

1. Share the Railway URL with team members
2. Add more features as needed
3. Monitor application logs regularly
4. Keep code updated on GitHub
5. Scale resources if usage increases

## Support

- **Railway Docs**: https://docs.railway.app
- **Supabase Docs**: https://supabase.com/docs
- **ASP.NET Core Docs**: https://docs.microsoft.com/aspnet/core
