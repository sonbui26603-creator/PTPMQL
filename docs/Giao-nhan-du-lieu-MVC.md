# Gửi và nhận dữ liệu giữa Model, View và Controller trong .NET MVC

## 1. Vai trò của Model, View và Controller

- **Model** biểu diễn dữ liệu và có thể chứa quy tắc kiểm tra dữ liệu. Ví dụ trong dự án là lớp `SanPham`, gồm mã, tên, giá và số lượng.
- **View** là giao diện Razor (`.cshtml`) hiển thị dữ liệu và tạo form để người dùng nhập dữ liệu.
- **Controller** nhận HTTP request, gọi xử lý dữ liệu, chọn View và truyền dữ liệu cần hiển thị. Khi nhận form, controller nhận dữ liệu đã được model binding gán vào tham số action.

Luồng phổ biến:

```text
Trình duyệt --request--> Controller --Model/dữ liệu--> View
Trình duyệt <--HTML------ Controller <--form data----- View
```

Trong MVC, View không gọi trực tiếp Controller như một hàm. View gửi HTTP request (thường qua liên kết hoặc form); routing chuyển request đến action phù hợp.

## 2. Truyền dữ liệu từ Controller sang View bằng ViewBag

Với dữ liệu nhỏ, chỉ dùng để hiển thị trong một request, có thể truyền bằng `ViewBag` (hoặc `ViewData`).

**Controller:**

```csharp
public IActionResult GioiThieu()
{
    ViewBag.TieuDe = "Thông tin sản phẩm";
    ViewBag.ThongBao = "Chào mừng bạn đến cửa hàng.";
    return View();
}
```

**View** `Views/SanPham/GioiThieu.cshtml`:

```cshtml
<h1>@ViewBag.TieuDe</h1>
<p>@ViewBag.ThongBao</p>
```

`ViewBag` thuận tiện cho dữ liệu phụ, nhưng là dynamic, không kiểm tra kiểu dữ liệu lúc biên dịch. Với dữ liệu chính của trang, nên truyền Model có kiểu rõ ràng.

## 3. Truyền dữ liệu từ Controller sang View thông qua Model

Controller truyền đối tượng khi gọi `View(model)`. View khai báo kiểu nhận bằng `@model`.

**Model:**

```csharp
public class SanPham
{
    public int Id { get; set; }
    public string TenSanPham { get; set; } = string.Empty;
    public decimal Gia { get; set; }
    public int SoLuong { get; set; }
}
```

**Controller:**

```csharp
public IActionResult Details()
{
    var sanPham = new SanPham
    {
        Id = 1,
        TenSanPham = "Bàn phím",
        Gia = 500000,
        SoLuong = 10
    };

    return View(sanPham);
}
```

**View** `Views/SanPham/Details.cshtml`:

```cshtml
@model DemoMVC.Models.SanPham

<h1>@Model.TenSanPham</h1>
<p>Giá: @Model.Gia</p>
<p>Số lượng: @Model.SoLuong</p>
```

Trong ví dụ thực tế của dự án, action `Index` truy vấn danh sách và trả về `View(sanPhams)`. View `Index.cshtml` khai báo `@model IEnumerable<DemoMVC.Models.SanPham>` rồi duyệt danh sách để hiển thị. Xem [SanPhamController.cs](../Controllers/SanPhamController.cs), [SanPham.cs](../Models/SanPham.cs) và [Views/SanPham/Index.cshtml](../Views/SanPham/Index.cshtml).

## 4. Nhận dữ liệu từ View trong Controller qua form và Model

Khi người dùng gửi form bằng `POST`, ASP.NET Core model binding đọc các trường form và gán chúng vào tham số action. Tên các trường được sinh bởi `asp-for` khớp với thuộc tính của model.

**View** `Views/SanPham/Create.cshtml`:

```cshtml
@model DemoMVC.Models.SanPham

<form asp-action="Create" method="post">
    <label asp-for="TenSanPham"></label>
    <input asp-for="TenSanPham" />
    <span asp-validation-for="TenSanPham"></span>

    <label asp-for="Gia"></label>
    <input asp-for="Gia" />

    <label asp-for="SoLuong"></label>
    <input asp-for="SoLuong" />

    <button type="submit">Lưu</button>
</form>
```

**Controller:**

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(SanPham sanPham)
{
    if (!ModelState.IsValid)
        return View(sanPham);

    _context.SanPhams.Add(sanPham);
    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(Index));
}
```

Ở đây, View gửi `TenSanPham`, `Gia` và `SoLuong`; model binding tạo đối tượng `SanPham sanPham`; controller kiểm tra validation rồi lưu. Nếu dữ liệu không hợp lệ, `return View(sanPham)` gửi lại dữ liệu đã nhập để View hiển thị cùng lỗi validation. Khi thành công, controller chuyển hướng về danh sách.

Đây cũng là luồng đang được dùng trong [Create.cshtml](../Views/SanPham/Create.cshtml) và action `Create` của [SanPhamController.cs](../Controllers/SanPhamController.cs). Các thuộc tính validation như `[Required]` và `[Range]` được khai báo trên [SanPham.cs](../Models/SanPham.cs).

## 5. Tóm tắt luồng gửi nhận

1. Trình duyệt gửi `GET` đến action; controller lấy hoặc tạo dữ liệu.
2. Controller trả `View(model)`; View nhận dữ liệu qua `@model` và hiển thị bằng `@Model`.
3. Người dùng điền form; các input được đặt tên theo thuộc tính model, thường qua `asp-for`.
4. Trình duyệt gửi form bằng `POST`; model binding đưa dữ liệu vào tham số action.
5. Controller kiểm tra `ModelState`, xử lý thành công hoặc trả lại View kèm lỗi.

Nên dùng Model có kiểu rõ ràng cho dữ liệu chính của trang; dùng `ViewBag`/`ViewData` cho dữ liệu phụ. Với form, dùng validation phía server và token chống giả mạo request (`[ValidateAntiForgeryToken]`).
