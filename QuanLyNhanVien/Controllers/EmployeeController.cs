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

    // Danh sách
    public IActionResult Index(string searchString, string khuVuc)
    {
        var employees = _context.Employees.AsQueryable();

        if (!string.IsNullOrEmpty(khuVuc))
        {
            employees = employees.Where(x =>
                x.KhuVuc == khuVuc);
        }

        if (!string.IsNullOrEmpty(searchString))
        {
            employees = employees.Where(x =>
                x.HoTen.Contains(searchString) ||
                x.MaNhanVien.Contains(searchString));
        }

        return View(employees.ToList());
    }

    // CREATE

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        Employee emp,
        IFormFile? imageFile)
    {
        emp.Ngay = DateTime.Today;

        if (imageFile != null && imageFile.Length > 0)
        {
            string fileName =
            Guid.NewGuid().ToString() +
            Path.GetExtension(imageFile.FileName);

            string uploadFolder =
            Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads");

            Directory.CreateDirectory(uploadFolder);

            string filePath =
            Path.Combine(
            uploadFolder,
            fileName);

            using(var stream =
                  new FileStream(
                  filePath,
                  FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            emp.ImagePath =
            "/uploads/" + fileName;
        }

        _context.Add(emp);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // EDIT

    public IActionResult Edit(int id)
    {
        var employee =
        _context.Employees.Find(id);

        if(employee==null)
            return NotFound();

        return View(employee);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(
        int id,
        Employee emp,
        IFormFile? imageFile)
    {
        if(id != emp.Id)
            return BadRequest();

        var existingEmployee =
        _context.Employees.Find(id);

        if(existingEmployee==null)
            return NotFound();

        existingEmployee.HoTen =
        emp.HoTen;

        existingEmployee.MaNhanVien =
        emp.MaNhanVien;

        existingEmployee.KhuVuc =
        emp.KhuVuc;

        existingEmployee.GhiChu =
        emp.GhiChu;

        existingEmployee.Latitude =
        emp.Latitude;

        existingEmployee.Longitude =
        emp.Longitude;

        existingEmployee.NgayCapNhat =
        DateTime.Today;

        // cập nhật ảnh mới

        if(imageFile != null &&
           imageFile.Length > 0)
        {
            string fileName =
            Guid.NewGuid().ToString() +
            Path.GetExtension(imageFile.FileName);

            string uploadFolder =
            Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads");

            Directory.CreateDirectory(uploadFolder);

            string filePath =
            Path.Combine(
            uploadFolder,
            fileName);

            using(var stream =
                  new FileStream(
                  filePath,
                  FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            existingEmployee.ImagePath =
            "/uploads/" + fileName;
        }

        _context.Update(existingEmployee);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // DELETE

    public IActionResult Delete(int id)
    {
        var employee =
        _context.Employees.Find(id);

        if(employee==null)
            return NotFound();

        return View(employee);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(
        int id)
    {
        var employee =
        _context.Employees.Find(id);

        if(employee != null)
        {
            _context.Remove(employee);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }

    // EXPORT EXCEL

    public IActionResult ExportExcel()
    {
        var employees =
        _context.Employees.ToList();

        using var package =
        new ExcelPackage();

        var worksheet =
        package.Workbook.Worksheets
        .Add("NhanVien");

        worksheet.Cells[1,1].Value="Họ tên";
        worksheet.Cells[1,2].Value="Mã NV";
        worksheet.Cells[1,3].Value="Khu vực";
        worksheet.Cells[1,4].Value="Ngày tạo";
        worksheet.Cells[1,5].Value="Ngày sửa";
        worksheet.Cells[1,6].Value="Hình";

        int row=2;

        foreach(var emp in employees)
        {
            worksheet.Cells[row,1].Value=emp.HoTen;
            worksheet.Cells[row,2].Value=emp.MaNhanVien;
            worksheet.Cells[row,3].Value=emp.KhuVuc;
            worksheet.Cells[row,4].Value=
            emp.Ngay.ToString("dd/MM/yyyy");

            worksheet.Cells[row,5].Value=
            emp.NgayCapNhat?.ToString("dd/MM/yyyy");

            // thêm ảnh

            if(!string.IsNullOrEmpty(emp.ImagePath))
            {
                try
                {
                    string imagePath =
                    Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    emp.ImagePath.TrimStart('/'));

                    if(System.IO.File.Exists(imagePath))
                    {
                        worksheet.Row(row).Height = 60;

                        var picture =
                        worksheet.Drawings.AddPicture(
                        $"Image_{emp.Id}",
                        new FileInfo(imagePath));

                        picture.SetPosition(
                        row-1,
                        5,
                        5,
                        5);

                        picture.SetSize(60,60);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            row++;
        }

        worksheet.Column(1).Width=25;
        worksheet.Column(2).Width=20;
        worksheet.Column(3).Width=20;
        worksheet.Column(4).Width=20;
        worksheet.Column(5).Width=20;
        worksheet.Column(6).Width=20;

        var bytes =
        package.GetAsByteArray();

        return File(
        bytes,
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "NhanVien.xlsx");
    }
}