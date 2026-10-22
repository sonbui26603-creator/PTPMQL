
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models
{
    public class SinhVien
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Không được để trống họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Không được để trống email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Range(18, 100, ErrorMessage = "Tuổi phải từ 18 đến 100")]
        public int Tuoi { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khoa")]
        public int MaKhoa { get; set; }

        [ForeignKey(nameof(MaKhoa))]
        public Khoa? Khoa { get; set; }
    }
}
