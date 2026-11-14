using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMVC.Migrations;

public partial class AddStudentFacultyAndCodeSequence : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Khoas",
            columns: table => new
            {
                MaKhoa = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                TenKhoa = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Khoas", x => x.MaKhoa);
            });

        migrationBuilder.CreateTable(
            name: "SinhVienCodeSequences",
            columns: table => new
            {
                NamNhapHoc = table.Column<int>(type: "int", nullable: false),
                MaHeDaoTao = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                MaNganh = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                SoThuTuCuoi = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey(
                    "PK_SinhVienCodeSequences",
                    x => new { x.NamNhapHoc, x.MaHeDaoTao, x.MaNganh });
            });

        migrationBuilder.CreateTable(
            name: "SinhViens",
            columns: table => new
            {
                MaSinhVien = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                HoTen = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                NamNhapHoc = table.Column<int>(type: "int", nullable: false),
                MaHeDaoTao = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                MaNganh = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                MaKhoa = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_SinhViens", x => x.MaSinhVien);
                table.ForeignKey(
                    name: "FK_SinhViens_Khoas_MaKhoa",
                    column: x => x.MaKhoa,
                    principalTable: "Khoas",
                    principalColumn: "MaKhoa",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_SinhViens_MaKhoa",
            table: "SinhViens",
            column: "MaKhoa");

        migrationBuilder.InsertData(
            table: "Khoas",
            columns: new[] { "MaKhoa", "TenKhoa" },
            values: new object[,]
            {
                { "CNTT", "Khoa Công nghệ thông tin" },
                { "DCK", "Khoa Cơ - Điện" },
                { "DKS", "Khoa Dầu khí" },
                { "DGC", "Khoa Địa chất" },
                { "DMT", "Khoa Môi trường" },
                { "DMO", "Khoa Mỏ" },
                { "DCC", "Khoa Xây dựng" },
                { "KTQTKD", "Khoa Kinh tế và Quản trị kinh doanh" },
                { "TDBD", "Khoa Trắc địa - Bản đồ và Quản lý đất đai" },
                { "KHTC", "Khoa Khoa học cơ bản" }
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "SinhViens");
        migrationBuilder.DropTable(name: "SinhVienCodeSequences");
        migrationBuilder.DropTable(name: "Khoas");
    }
}
