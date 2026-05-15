using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuanLyNhanVien.Data;
using QuanLyNhanVien.Hubs;
using QuanLyNhanVien.Models;
using System.IO;
using System.Linq;
using OfficeOpenXml;

public class EmployeeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHubContext<NotificationHub> _hubContext;

    public EmployeeController(AppDbContext context, IWebHostEnvironment webHostEnvironment, IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _hubContext = hubContext;
    }

    public IActionResult Index(string searchString)
{
    var employees = _context.Employees.AsQueryable();

    // Tìm kiếm theo Họ tên hoặc Mã nhân viên
    if (!string.IsNullOrEmpty(searchString))
    {
        employees = employees.Where(e =>
            e.HoTen.Contains(searchString) ||
            e.MaNhanVien.Contains(searchString));
    }

    return View(employees.ToList());
}
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Employee emp, IFormFile? imageFile)
    {
        emp.Ngay = DateTime.UtcNow;

        // Xử lý upload hình ảnh
        if (imageFile != null && imageFile.Length > 0)
        {
            string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
            string extension = Path.GetExtension(imageFile.FileName);
            string newFileName = fileName + "_" + DateTime.UtcNow.Ticks + extension;
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", newFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            emp.ImagePath = "/uploads/" + newFileName;
        }

        _context.Add(emp);
        _context.SaveChanges();

        var locationText = emp.Latitude.HasValue && emp.Longitude.HasValue
            ? $"Vị trí: {emp.Latitude:F5}, {emp.Longitude:F5}" : "Vị trí: không xác định";
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Nhân viên mới nhập từ điện thoại: {emp.HoTen} | {emp.KhuVuc} | {locationText}");

        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var employee = _context.Employees.Find(id);
        if (employee == null)
            return NotFound();
        return View(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Employee emp, IFormFile? imageFile)
    {
        if (id != emp.Id)
            return BadRequest();

        var existingEmployee = _context.Employees.Find(id);
        if (existingEmployee == null)
            return NotFound();

        existingEmployee.HoTen = emp.HoTen;
        existingEmployee.MaNhanVien = emp.MaNhanVien;
        existingEmployee.GhiChu = emp.GhiChu;
        existingEmployee.KhuVuc = emp.KhuVuc;
        existingEmployee.Latitude = emp.Latitude;
        existingEmployee.Longitude = emp.Longitude;
existingEmployee.Ngay = DateTime.SpecifyKind(emp.Ngay, DateTimeKind.Utc);
        // Xử lý upload hình ảnh mới
        if (imageFile != null && imageFile.Length > 0)
        {
            string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
            string extension = Path.GetExtension(imageFile.FileName);
            string newFileName = fileName + "_" + DateTime.UtcNow.Ticks + extension;
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", newFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            existingEmployee.ImagePath = "/uploads/" + newFileName;
        }

        
        _context.Update(existingEmployee);
        _context.SaveChanges();

        var locationText = existingEmployee.Latitude.HasValue && existingEmployee.Longitude.HasValue
            ? $"Vị trí: {existingEmployee.Latitude:F5}, {existingEmployee.Longitude:F5}" : "Vị trí: không xác định";
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"Nhân viên đã cập nhật từ điện thoại: {existingEmployee.HoTen} | {existingEmployee.KhuVuc} | {locationText}");

        return RedirectToAction("Index");
    }

    public IActionResult Delete(int id)
    {
        var employee = _context.Employees.Find(id);
        if (employee == null)
            return NotFound();
        return View(employee);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var employee = _context.Employees.Find(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            _context.SaveChanges();
        }
        return RedirectToAction("Index");
    }

    public IActionResult ExportExcel()
    {
        var employees = _context.Employees.ToList();
        
        using (var package = new OfficeOpenXml.ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Nhân viên");

            // Header
            worksheet.Cells[1, 1].Value = "Họ tên";
            worksheet.Cells[1, 2].Value = "Mã NV";
            worksheet.Cells[1, 3].Value = "Khu vực";
            worksheet.Cells[1, 4].Value = "Ngày";
            worksheet.Cells[1, 5].Value = "Ghi chú";
            worksheet.Cells[1, 6].Value = "Hình ảnh";

            // Format header
            for (int i = 1; i <= 6; i++)
            {
                worksheet.Cells[1, i].Style.Font.Bold = true;
                worksheet.Cells[1, i].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells[1, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
            }

            // Data
            int row = 2;
            foreach (var emp in employees)
            {
                worksheet.Cells[row, 1].Value = emp.HoTen;
                worksheet.Cells[row, 2].Value = emp.MaNhanVien;
                worksheet.Cells[row, 3].Value = emp.KhuVuc;
                worksheet.Cells[row, 4].Value = emp.Ngay.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 5].Value = emp.GhiChu;

                // Insert image if available
                if (!string.IsNullOrEmpty(emp.ImagePath))
                {
                    try
                    {
                        string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, emp.ImagePath.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            var picture = worksheet.Drawings.AddPicture($"Image_{emp.Id}", new FileInfo(imagePath));
                            picture.From.Column = 5;
                            picture.From.Row = row - 1;
                            picture.SetSize(60, 60);
                        }
                    }
                    catch { }
                }

                row++;
            }

            // Auto-fit columns
            worksheet.Column(1).Width = 20;
            worksheet.Column(2).Width = 15;
            worksheet.Column(3).Width = 15;
            worksheet.Column(4).Width = 15;
            worksheet.Column(5).Width = 30;
            worksheet.Column(6).Width = 15;

            var fileName = $"DanhSachNhanVien_{DateTime.UtcNow:ddMMyyyy_HHmmss}.xlsx";
            var fileBytes = package.GetAsByteArray();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}