using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DemoMVC.ViewModels;

public class SinhVienCreateViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(150)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [Range(2000, 2099, ErrorMessage = "Năm nhập học phải từ 2000 đến 2099")]
    [Display(Name = "Năm nhập học")]
    public int NamNhapHoc { get; set; } = DateTime.Now.Year;

    [Required]
    [StringLength(2)]
    [RegularExpression(@"^\d{2}$", ErrorMessage = "Mã hệ đào tạo phải gồm đúng 2 chữ số")]
    [Display(Name = "Mã hệ đào tạo")]
    public string MaHeDaoTao { get; set; } = "21";

    [Required]
    [StringLength(2)]
    [RegularExpression(@"^\d{2}$", ErrorMessage = "Mã ngành phải gồm đúng 2 chữ số")]
    [Display(Name = "Mã ngành")]
    public string MaNganh { get; set; } = "05";

    [Required(ErrorMessage = "Vui lòng chọn khoa")]
    [Display(Name = "Khoa")]
    public string MaKhoa { get; set; } = string.Empty;

    public IReadOnlyList<SelectListItem> DanhSachKhoa { get; set; } = [];
}
