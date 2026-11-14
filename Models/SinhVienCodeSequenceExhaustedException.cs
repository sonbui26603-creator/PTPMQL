namespace DemoMVC.Models;

public sealed class SinhVienCodeSequenceExhaustedException : Exception
{
    public SinhVienCodeSequenceExhaustedException()
        : base("Đã dùng hết 9.999 số thứ tự cho năm nhập học, hệ đào tạo và ngành này.")
    {
    }
}
