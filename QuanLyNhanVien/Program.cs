using Microsoft.EntityFrameworkCore;
using QuanLyNhanVien.Data;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// EPPlus License
// ==========================
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// ==========================
// Add services
// ==========================
builder.Services.AddControllersWithViews();

// ==========================
// PostgreSQL Supabase
// ==========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// ==========================
// SignalR
// ==========================
builder.Services.AddSignalR();

var app = builder.Build();

// ==========================
// AUTO MIGRATION DATABASE
// ==========================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ==========================
// Configure pipeline
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    // HIỆN LỖI THẬT TRÊN RENDER
    app.UseDeveloperExceptionPage();

    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// HTTPS
app.UseHttpsRedirection();

// Static files
app.UseStaticFiles();

// Routing
app.UseRouting();

// Authorization
app.UseAuthorization();

// ==========================
// Mobile Middleware
// ==========================
app.UseMiddleware<QuanLyNhanVien.Middleware.MobileAccessMiddleware>();

// ==========================
// MVC Route
// ==========================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}");

// ==========================
// SignalR Hub
// ==========================
app.MapHub<QuanLyNhanVien.Hubs.NotificationHub>("/notificationHub");

// ==========================
// Run app
// ==========================
app.Run();