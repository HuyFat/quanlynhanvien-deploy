using Microsoft.EntityFrameworkCore;
using QuanLyNhanVien.Data;
using OfficeOpenXml;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// EPPlus license
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// MVC
builder.Services.AddControllersWithViews();

// DB (Supabase PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure();
        }));

// SignalR
builder.Services.AddSignalR();

// PORT Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

// Exception handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Custom middleware
app.UseMiddleware<QuanLyNhanVien.Middleware.MobileAccessMiddleware>();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}");

// SignalR hub
app.MapHub<QuanLyNhanVien.Hubs.NotificationHub>("/notificationHub");

app.Run();