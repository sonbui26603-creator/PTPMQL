using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models;

public class SinhVien
{
    [Key]
    [StringLength(10)]
    [Display(Name = "Mã sinh viên")]
    public string MaSinhVien { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(150)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [Range(2000, 2099, ErrorMessage = "Năm nhập học phải từ 2000 đến 2099")]
    [Display(Name = "Năm nhập học")]
    public int NamNhapHoc { get; set; }

    [Required]
    [StringLength(2)]
    [RegularExpression(@"^\d{2}$", ErrorMessage = "Mã hệ đào tạo phải gồm đúng 2 chữ số")]
    [Display(Name = "Mã hệ đào tạo")]
    public string MaHeDaoTao { get; set; } = string.Empty;

    [Required]
    [StringLength(2)]
    [RegularExpression(@"^\d{2}$", ErrorMessage = "Mã ngành phải gồm đúng 2 chữ số")]
    [Display(Name = "Mã ngành")]
    public string MaNganh { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    [Display(Name = "Khoa")]
    public string MaKhoa { get; set; } = string.Empty;

    public Khoa Khoa { get; set; } = null!;
}
