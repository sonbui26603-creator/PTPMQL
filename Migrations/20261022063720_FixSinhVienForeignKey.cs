using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMVC.Migrations
{
    /// <inheritdoc />
    public partial class FixSinhVienForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SinhViens_Khoas_KhoaMaKhoa",
                table: "SinhViens");

            migrationBuilder.DropIndex(
                name: "IX_SinhViens_KhoaMaKhoa",
                table: "SinhViens");

            migrationBuilder.DropColumn(
                name: "KhoaMaKhoa",
                table: "SinhViens");

            migrationBuilder.CreateIndex(
                name: "IX_SinhViens_MaKhoa",
                table: "SinhViens",
                column: "MaKhoa");

            migrationBuilder.AddForeignKey(
                name: "FK_SinhViens_Khoas_MaKhoa",
                table: "SinhViens",
                column: "MaKhoa",
                principalTable: "Khoas",
                principalColumn: "MaKhoa",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SinhViens_Khoas_MaKhoa",
                table: "SinhViens");

            migrationBuilder.DropIndex(
                name: "IX_SinhViens_MaKhoa",
                table: "SinhViens");

            migrationBuilder.AddColumn<int>(
                name: "KhoaMaKhoa",
                table: "SinhViens",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SinhViens_KhoaMaKhoa",
                table: "SinhViens",
                column: "KhoaMaKhoa");

            migrationBuilder.AddForeignKey(
                name: "FK_SinhViens_Khoas_KhoaMaKhoa",
                table: "SinhViens",
                column: "KhoaMaKhoa",
                principalTable: "Khoas",
                principalColumn: "MaKhoa");
        }
    }
}
