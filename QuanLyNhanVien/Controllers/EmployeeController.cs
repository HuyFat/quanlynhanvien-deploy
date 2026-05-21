using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuanLyNhanVien.Data;
using QuanLyNhanVien.Hubs;
using QuanLyNhanVien.Models;
using OfficeOpenXml;

public class EmployeeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IHubContext<NotificationHub> _hubContext;

    public EmployeeController(
        AppDbContext context,
        IWebHostEnvironment webHostEnvironment,
        IHubContext<NotificationHub> hubContext)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _hubContext = hubContext;
    }

    // DANH SÁCH + TÌM KIẾM
    public IActionResult Index(string searchString, string khuVuc)
    {
        var employees = _context.Employees.AsQueryable();

        // Lọc khu vực
        if (!string.IsNullOrEmpty(khuVuc))
        {
            employees = employees.Where(x =>
                x.KhuVuc == khuVuc);
        }

        // Tìm tên hoặc mã NV
        if (!string.IsNullOrEmpty(searchString))
        {
            employees = employees.Where(x =>
                x.HoTen.Contains(searchString) ||
                x.MaNhanVien.Contains(searchString));
        }

        return View(employees.ToList());
    }

    // TRANG THÊM
    public IActionResult Create()
    {
        return View();
    }

    // THÊM NHÂN VIÊN
    [HttpPost]
    public async Task<IActionResult> Create(
        Employee emp,
        IFormFile? imageFile)
    {
        // ngày tạo
        emp.Ngay = DateTime.Now;

        // upload ảnh
        if (imageFile != null && imageFile.Length > 0)
        {
            string fileName =
            Path.GetFileNameWithoutExtension(imageFile.FileName);

            string extension =
            Path.GetExtension(imageFile.FileName);

            string newFileName =
            fileName + "_" +
            DateTime.Now.Ticks +
            extension;

            string filePath =
            Path.Combine(
                _webHostEnvironment.WebRootPath,
                "uploads",
                newFileName);

            Directory.CreateDirectory(
                Path.GetDirectoryName(filePath)!);

            using (var stream =
                   new FileStream(
                       filePath,
                       FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            emp.ImagePath = "/uploads/" + newFileName;
        }

        _context.Add(emp);

        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync(
            "ReceiveNotification",
            $"Đã thêm: {emp.HoTen}");

        return RedirectToAction(nameof(Index));
    }

    // TRANG SỬA
    public IActionResult Edit(int id)
    {
        var employee =
            _context.Employees.Find(id);

        if (employee == null)
            return NotFound();

        return View(employee);
    }

    // SỬA
    [HttpPost]
    public async Task<IActionResult> Edit(
        int id,
        Employee emp,
        IFormFile? imageFile)
    {
        if (id != emp.Id)
            return BadRequest();

        var existingEmployee =
            _context.Employees.Find(id);

        if (existingEmployee == null)
            return NotFound();

        existingEmployee.HoTen =
            emp.HoTen;

        existingEmployee.MaNhanVien =
            emp.MaNhanVien;

        existingEmployee.GhiChu =
            emp.GhiChu;

        existingEmployee.KhuVuc =
            emp.KhuVuc;

        existingEmployee.Latitude =
            emp.Latitude;

        existingEmployee.Longitude =
            emp.Longitude;


        // NGÀY CHỈNH SỬA

        existingEmployee.NgayCapNhat =
            DateTime.Now;

        // upload ảnh mới

        if (imageFile != null &&
            imageFile.Length > 0)
        {
            string fileName =
            Path.GetFileNameWithoutExtension(
                imageFile.FileName);

            string extension =
            Path.GetExtension(
                imageFile.FileName);

            string newFileName =
            fileName + "_" +
            DateTime.Now.Ticks +
            extension;

            string filePath =
            Path.Combine(
                _webHostEnvironment.WebRootPath,
                "uploads",
                newFileName);

            Directory.CreateDirectory(
                Path.GetDirectoryName(filePath)!);

            using (var stream =
                   new FileStream(
                       filePath,
                       FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            existingEmployee.ImagePath =
                "/uploads/" + newFileName;
        }

        _context.Update(existingEmployee);

        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync(
            "ReceiveNotification",
            $"Đã cập nhật: {existingEmployee.HoTen}");

        return RedirectToAction(nameof(Index));
    }

    // XÓA
    public IActionResult Delete(int id)
    {
        var employee =
            _context.Employees.Find(id);

        if (employee == null)
            return NotFound();

        return View(employee);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id)
    {
        var employee =
            _context.Employees.Find(id);

        if (employee != null)
        {
            _context.Remove(employee);
            _context.SaveChanges();
        }

        return RedirectToAction(nameof(Index));
    }

    // XUẤT EXCEL
    public IActionResult ExportExcel()
    {
        var employees =
            _context.Employees.ToList();

        using var package =
            new ExcelPackage();

        var worksheet =
            package.Workbook.Worksheets.Add("NhanVien");

        worksheet.Cells[1,1].Value="Họ tên";
        worksheet.Cells[1,2].Value="Mã NV";
        worksheet.Cells[1,3].Value="Khu vực";
        worksheet.Cells[1,4].Value="Ngày tạo";
        worksheet.Cells[1,5].Value="Ngày sửa";

        int row=2;

        foreach(var emp in employees)
        {
            worksheet.Cells[row,1].Value=emp.HoTen;
            worksheet.Cells[row,2].Value=emp.MaNhanVien;
            worksheet.Cells[row,3].Value=emp.KhuVuc;
            worksheet.Cells[row,4].Value=
            emp.Ngay.ToString("dd/MM/yyyy");

            worksheet.Cells[row,5].Value=
            emp.NgayCapNhat?.ToString("dd/MM/yyyy HH:mm");

            row++;
        }

        var bytes=
        package.GetAsByteArray();

        return File(
            bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "NhanVien.xlsx");
    }
}