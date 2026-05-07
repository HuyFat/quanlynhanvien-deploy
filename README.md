# QuanLyNhanVien - Employee Management System

## Overview
Web application for managing employees built with ASP.NET Core 9.0 and PostgreSQL (Supabase).

## Tech Stack
- **Framework**: ASP.NET Core 9.0
- **Database**: PostgreSQL (Supabase)
- **ORM**: Entity Framework Core 8.0
- **Real-time**: SignalR
- **Frontend**: HTML/CSS/JavaScript with Bootstrap

## Prerequisites
- .NET 9.0 SDK
- PostgreSQL connection string from Supabase

## Setup Local Development

### 1. Clone the repository
```bash
git clone <repository-url>
cd thuctap
```

### 2. Update database connection string
Edit `QuanLyNhanVien/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-host.supabase.co;Database=postgres;Username=postgres;Password=your-password;Port=5432;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

### 3. Restore dependencies
```bash
cd QuanLyNhanVien
dotnet restore
```

### 4. Apply migrations (create database tables)
```bash
dotnet ef database update
```

### 5. Run the application
```bash
dotnet run
```

Application will start at: `http://localhost:5262`

## Deployment to Railway

### 1. Push code to GitHub
```bash
git init
git add .
git commit -m "Initial commit"
git remote add origin <your-github-repo>
git push -u origin main
```

### 2. Deploy on Railway
1. Go to [railway.app](https://railway.app)
2. Sign up with GitHub account
3. Create new project > "Deploy from GitHub repo"
4. Select your repository
5. Railway will auto-detect .NET and build automatically

### 3. Configure environment variables
In Railway Project Settings > Variables, add:
- Key: `ConnectionStrings__DefaultConnection`
- Value: `Host=your-host.supabase.co;Database=postgres;Username=postgres;Password=your-password;Port=5432;SSL Mode=Require;Trust Server Certificate=true`

### 4. Deploy
Railway will automatically build and deploy. Once complete, you'll get a public URL.

## Access from Mobile
Once deployed to Railway, the application URL is accessible from any device on any network:
- Example: `https://your-app-name.up.railway.app`
- Mobile users can access by typing this URL in their browser

## Project Structure
```
QuanLyNhanVien/
├── Controllers/          # MVC Controllers
├── Data/                 # Database context
├── Models/               # Data models
├── Views/                # Razor views (HTML)
├── wwwroot/              # Static files (CSS, JS)
├── Middleware/           # Custom middleware
├── Hubs/                 # SignalR hubs
├── Migrations/           # EF Core migrations
├── Program.cs            # Application entry point
├── appsettings.json      # Configuration
└── QuanLyNhanVien.csproj # Project file
```

## Database Schema
### Employees Table
- EmployeeId (int) - Primary Key
- Name (string) - Employee name
- Position (string) - Job position
- Department (string) - Department
- HireDate (DateTime) - Hire date
- Salary (decimal) - Salary
- PhoneNumber (string) - Contact phone
- Address (string) - Address
- Latitude (decimal) - Location latitude
- Longitude (decimal) - Location longitude

## Features
- ✅ Employee CRUD operations
- ✅ Mobile device detection
- ✅ Real-time notifications via SignalR
- ✅ Location tracking (latitude/longitude)
- ✅ Employee data export to Excel
- ✅ Responsive design

## Troubleshooting

### Cannot connect to database
- Verify connection string is correct
- Check Supabase firewall settings allow connection
- Ensure password is URL-encoded if it contains special characters

### Build errors
```bash
dotnet clean
dotnet restore
dotnet build
```

### Migration errors
```bash
dotnet ef database drop  # WARNING: This deletes all data
dotnet ef database update
```

## Security Notes
⚠️ Never commit `appsettings.json` with real passwords to GitHub.
Use Railway/Azure environment variables for sensitive data instead.

## License
MIT

## Contact
For issues or questions, please create a GitHub issue.
