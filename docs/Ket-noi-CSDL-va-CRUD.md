# Kết nối cơ sở dữ liệu và xây dựng CRUD trong ASP.NET Core MVC

CRUD là bốn thao tác cơ bản với dữ liệu:

- **Create**: thêm bản ghi.
- **Read**: đọc danh sách hoặc chi tiết.
- **Update**: sửa bản ghi.
- **Delete**: xóa bản ghi.

Dự án này dùng Entity Framework Core (EF Core) 10 và SQL Server. Entity `SanPham`, `AppDbContext`, migration `InitialCreate` và các action CRUD đã có sẵn; các bước dưới đây giải thích cấu hình và cách phát triển tiếp.

## 1. Cấu hình kết nối cơ sở dữ liệu

Connection string có thể khai báo trong `appsettings.json`, nhưng không nên commit mật khẩu thật vào Git. Trong môi trường phát triển, có thể lưu chuỗi kết nối bằng User Secrets:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=DemoMVC;Trusted_Connection=True;TrustServerCertificate=True;"
```

Nếu dùng SQL Server Authentication, cung cấp `User Id` và `Password` qua User Secrets hoặc biến môi trường; không đưa thông tin xác thực thật vào source control. Trên production có thể đặt biến môi trường:

```text
ConnectionStrings__DefaultConnection
```

Hai dấu gạch dưới `__` được ánh xạ thành dấu `:` trong cấu hình ASP.NET Core.

Đăng ký context với DI trong `Program.cs`:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
```

`AppDbContext` kế thừa `DbContext`, nhận `DbContextOptions<AppDbContext>` qua constructor và khai báo mỗi bảng qua một `DbSet<TEntity>`. Dự án đăng ký context theo scope của request HTTP.

## 2. Tạo Entity và DbContext

Entity thường là lớp C# mô tả một loại dữ liệu. Ví dụ entity `SanPham` trong dự án:

```csharp
public class SanPham
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TenSanPham { get; set; } = string.Empty;

    [Range(0.01, 1000000000)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Gia { get; set; }

    [Range(0, 1000000)]
    public int SoLuong { get; set; }
}
```

Các Data Annotations hỗ trợ validation và cấu hình schema. `AppDbContext` khai báo `DbSet<SanPham> SanPhams`, nhờ đó EF Core ánh xạ entity thành bảng `SanPhams`.

## 3. Dùng Migrations quản lý phiên bản schema

Migration lưu các thay đổi schema dưới dạng mã nguồn có thể kiểm tra và áp dụng lên database. Dự án đã có migration `InitialCreate` tạo bảng `SanPhams`; không cần tạo lại migration đầu tiên.

Cài EF CLI một lần nếu máy chưa có, sau đó chạy lệnh từ thư mục chứa `DemoMVC.csproj`:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.0
dotnet ef migrations list
dotnet ef database update
```

Khi thay đổi entity, tạo migration mới với tên mô tả thay đổi rồi cập nhật database:

```powershell
dotnet ef migrations add AddCategoryToSanPham
dotnet ef database update
```

Không xóa hoặc sửa migration đã được áp dụng trên môi trường dùng chung nếu chưa đánh giá tác động. Cần kiểm tra migration được sinh ra trước khi cập nhật database.

## 4. Xây dựng CRUD

Controller `SanPhamController` dùng `AppDbContext` để thao tác dữ liệu. Các mẫu thao tác trong dự án:

```csharp
// Read: lấy danh sách
var sanPhams = await _context.SanPhams
    .AsNoTracking()
    .ToListAsync();

// Create: thêm mới
_context.SanPhams.Add(sanPham);
await _context.SaveChangesAsync();

// Update: cập nhật
_context.SanPhams.Update(sanPham);
await _context.SaveChangesAsync();

// Delete: xóa
_context.SanPhams.Remove(sanPham);
await _context.SaveChangesAsync();
```

Mỗi thao tác ghi dữ liệu phải gọi `SaveChangesAsync`. Với các action nhận form `POST`, dự án dùng `[ValidateAntiForgeryToken]`, kiểm tra `ModelState.IsValid` trước khi lưu và redirect sau khi thành công. Việc redirect sau POST tránh trình duyệt gửi lại form khi refresh.

Các view MVC được tìm theo tên action trong `Views/{Controller}/{Action}.cshtml`. Ví dụ controller có action `Edit` thì view thường nằm tại `Views/SanPham/Edit.cshtml`. Đã có thể dùng danh sách và thực hiện đủ các thao tác Create, Read, Update, Delete với các view trong `Views/SanPham`.

## 5. Sinh mã CRUD bằng scaffolding

Scaffolding có thể sinh controller và view CRUD từ entity cùng `DbContext`. Dự án hiện đã có `SanPhamController` và các view CRUD, vì vậy không chạy lệnh scaffold dưới đây trực tiếp với `SanPhamController` để tránh ghi đè hoặc tạo class trùng.

Với entity mới trong dự án ASP.NET Core MVC, cài package scaffolding phù hợp với target framework và cài công cụ nếu chưa có:

```powershell
dotnet tool install --global dotnet-aspnet-codegenerator --version 10.0.0
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design --version 10.0.0
```

Sau đó chạy từ thư mục project:

```powershell
dotnet aspnet-codegenerator controller `
  -name DanhMucController `
  -m DanhMuc `
  -dc AppDbContext `
  --relativeFolderPath Controllers `
  --useDefaultLayout `
  --referenceScriptLibraries
```

Thay `DanhMuc` bằng entity thực tế. Kiểm tra mã được sinh, đặc biệt validation, bảo vệ request POST, quyền truy cập và logic cập nhật trước khi sử dụng.

## 6. Các file liên quan trong dự án

- [Program.cs](../Program.cs): đăng ký dịch vụ MVC, SQL Server và route.
- [Data/AppDbContext.cs](../Data/AppDbContext.cs): khai báo EF Core context.
- [Models/SanPham.cs](../Models/SanPham.cs): entity và validation.
- [Migrations/](../Migrations/): lịch sử thay đổi schema.
- [Controllers/SanPhamController.cs](../Controllers/SanPhamController.cs): các action CRUD.
- [Views/SanPham/](../Views/SanPham/): giao diện danh sách, tạo, chi tiết, sửa và xóa sản phẩm.
