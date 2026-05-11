using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace QuanLyNhanVien.Models;

public class Employee
{
    [Key]
[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
public int Id { get; set; }
    public required string HoTen { get; set; }
    public required string MaNhanVien { get; set; }
    public string? GhiChu { get; set; }
    public string? KhuVuc { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime Ngay { get; set; }
    public string? ImagePath { get; set; }
}
