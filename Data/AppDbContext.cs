
using Microsoft.EntityFrameworkCore;
using DemoMVC.Models;

namespace DemoMVC.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<SanPham> SanPhams => Set<SanPham>();

    public DbSet<Khoa> Khoas => Set<Khoa>();

    public DbSet<SinhVien> SinhViens => Set<SinhVien>();

    public DbSet<SinhVienCodeSequence> SinhVienCodeSequences => Set<SinhVienCodeSequence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SinhVien>()
            .HasOne(sinhVien => sinhVien.Khoa)
            .WithMany(khoa => khoa.SinhViens)
            .HasForeignKey(sinhVien => sinhVien.MaKhoa)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SinhVienCodeSequence>()
            .Property(sequence => sequence.MaHeDaoTao)
            .HasMaxLength(2);

        modelBuilder.Entity<SinhVienCodeSequence>()
            .Property(sequence => sequence.MaNganh)
            .HasMaxLength(2);

        modelBuilder.Entity<SinhVienCodeSequence>()
            .HasKey(sequence => new
            {
                sequence.NamNhapHoc,
                sequence.MaHeDaoTao,
                sequence.MaNganh
            });

        modelBuilder.Entity<Khoa>().HasData(
            new Khoa { MaKhoa = "CNTT", TenKhoa = "Khoa Công nghệ thông tin" },
            new Khoa { MaKhoa = "DCK", TenKhoa = "Khoa Cơ - Điện" },
            new Khoa { MaKhoa = "DKS", TenKhoa = "Khoa Dầu khí" },
            new Khoa { MaKhoa = "DGC", TenKhoa = "Khoa Địa chất" },
            new Khoa { MaKhoa = "DMT", TenKhoa = "Khoa Môi trường" },
            new Khoa { MaKhoa = "DMO", TenKhoa = "Khoa Mỏ" },
            new Khoa { MaKhoa = "DCC", TenKhoa = "Khoa Xây dựng" },
            new Khoa { MaKhoa = "KTQTKD", TenKhoa = "Khoa Kinh tế và Quản trị kinh doanh" },
            new Khoa { MaKhoa = "TDBD", TenKhoa = "Khoa Trắc địa - Bản đồ và Quản lý đất đai" },
            new Khoa { MaKhoa = "KHTC", TenKhoa = "Khoa Khoa học cơ bản" });
    }
}
