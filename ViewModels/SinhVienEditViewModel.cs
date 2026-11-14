using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DemoMVC.ViewModels;

public class SinhVienEditViewModel
{
    public string MaSinhVien { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(150)]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn khoa")]
    [Display(Name = "Khoa")]
    public string MaKhoa { get; set; } = string.Empty;

    public IReadOnlyList<SelectListItem> DanhSachKhoa { get; set; } = [];
}
