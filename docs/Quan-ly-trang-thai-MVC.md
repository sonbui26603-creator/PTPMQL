# Quản lý trạng thái trong ASP.NET Core MVC

HTTP là giao thức không lưu trạng thái giữa các request. Vì vậy, khi cần chuyển dữ liệu trong lúc xử lý một request hoặc sau khi chuyển hướng, MVC cung cấp các cơ chế như `ViewBag`, `ViewData` và `TempData`.

## 1. ViewBag

`ViewBag` là cách truy cập dữ liệu động, dễ viết để truyền thông tin từ Controller sang View trong cùng một request.

**Controller:**

```csharp
public IActionResult Index()
{
    ViewBag.TieuDe = "Danh sách sản phẩm";
    ViewBag.ThongBao = "Chào mừng bạn!";
    return View();
}
```

**View:**

```cshtml
<h1>@ViewBag.TieuDe</h1>
<p>@ViewBag.ThongBao</p>
```

`ViewBag` không tồn tại qua request tiếp theo. Nếu action trả về redirect, dữ liệu `ViewBag` không được mang sang action đích. Vì là dynamic, sai tên thuộc tính thường chỉ được phát hiện khi chạy.

## 2. ViewData

`ViewData` là dictionary dùng để truyền dữ liệu từ Controller sang View trong cùng một request. Khóa là chuỗi và giá trị có kiểu `object`.

**Controller:**

```csharp
public IActionResult Privacy()
{
    ViewData["TieuDe"] = "Chính sách riêng tư";
    ViewData["SoMuc"] = 3;
    return View();
}
```

**View:**

```cshtml
<h1>@ViewData["TieuDe"]</h1>
<p>Số mục: @ViewData["SoMuc"]</p>
```

`ViewBag` và `ViewData` cùng sử dụng dữ liệu của `ViewData`; chúng là hai cách truy cập thuận tiện khác nhau vào dữ liệu cho View. Dữ liệu không được giữ sau redirect. Khi lấy dữ liệu trong C#, cần kiểm tra hoặc chuyển kiểu phù hợp vì giá trị được lưu dưới dạng `object`.

## 3. TempData

`TempData` dùng để chuyển dữ liệu tạm thời sang request kế tiếp, thường là thông báo sau khi thực hiện thao tác rồi redirect (mẫu Post/Redirect/Get).

**Controller:**

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Save()
{
    // Thực hiện lưu dữ liệu ở đây.
    TempData["ThongBao"] = "Đã lưu thành công.";
    return RedirectToAction(nameof(Index));
}
```

**Action đích:**

```csharp
public IActionResult Index()
{
    return View();
}
```

**View `Index.cshtml`:**

```cshtml
@if (TempData["ThongBao"] is string thongBao)
{
    <div class="alert alert-success">@thongBao</div>
}
```

Khi đọc một giá trị `TempData`, MVC đánh dấu giá trị đó để xóa sau khi request hoàn tất. Có thể dùng `Peek` để đọc mà không đánh dấu xóa:

```csharp
var thongBao = TempData.Peek("ThongBao");
```

Hoặc dùng `Keep` để giữ lại giá trị đã được đọc:

```csharp
var thongBao = TempData["ThongBao"];
TempData.Keep("ThongBao");
```

Trong ASP.NET Core MVC, TempData thường được lưu qua cookie bởi `CookieTempDataProvider` mặc định; cookie được bảo vệ bởi cơ chế Data Protection của ASP.NET Core. TempData phù hợp với dữ liệu nhỏ, tạm thời, không phải nơi lưu trữ dữ liệu nghiệp vụ hoặc dữ liệu lớn.

## 4. So sánh nhanh

| Cơ chế | Cách truy cập | Phạm vi tồn tại | Trường hợp sử dụng |
|---|---|---|---|
| `ViewBag` | Thuộc tính động | Request hiện tại | Thông tin phụ, ngắn gọn để hiển thị trong View |
| `ViewData` | Dictionary khóa chuỗi | Request hiện tại | Thông tin phụ cần truyền sang View |
| `TempData` | Dictionary khóa chuỗi | Request kế tiếp; mặc định bị xóa sau khi đọc | Thông báo sau redirect |

## 5. Chọn cơ chế phù hợp

- Dùng **Model** cho dữ liệu chính của trang, có cấu trúc và cần kiểm tra kiểu.
- Dùng **ViewBag** hoặc **ViewData** cho thông tin phụ chỉ cần trong request hiện tại.
- Dùng **TempData** cho thông báo ngắn sau redirect.
- Nếu cần lưu trạng thái lâu hơn giữa nhiều request, hãy chọn cơ chế phù hợp với yêu cầu như database, cache hoặc session; không coi `ViewBag`, `ViewData` hay `TempData` là kho dữ liệu lâu dài.
