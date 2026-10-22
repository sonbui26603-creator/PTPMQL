
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class Khoa
    {
        [Key]
        public int MaKhoa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khoa")]
        public string TenKhoa { get; set; } = string.Empty;

        public ICollection<SinhVien> SinhViens { get; set; }
            = new List<SinhVien>();
    }
}
