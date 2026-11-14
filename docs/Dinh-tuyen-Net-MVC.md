# Tìm hiểu định tuyến trong .NET MVC

Định tuyến (routing) là cơ chế chọn endpoint phù hợp với URL và HTTP method của request. Trong MVC, endpoint thường là một action trong controller. Sau khi tìm được action, ASP.NET Core bind dữ liệu từ URL, query string hoặc request body vào tham số rồi thực thi action.

## 1. Cấu trúc URL trong MVC

Một URL có thể gồm các phần:

```text
https://example.com/{controller}/{action}/{id}?keyword=banh
```

Ví dụ:

```text
https://example.com/SanPham/Details/5?keyword=banh
```

- `SanPham` là tên controller (lớp `SanPhamController`).
- `Details` là tên action.
- `5` là giá trị của tham số `id`.
- `?keyword=banh` là query string; action có thể nhận giá trị này qua tham số `keyword`.

### Conventional routing

Dự án này khai báo route mặc định trong `Program.cs`:

```csharp
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
```

Các phần trong mẫu route:

- `{controller=Home}`: mặc định chọn `HomeController` nếu URL không có tên controller.
- `{action=Index}`: mặc định chọn action `Index` nếu URL không có tên action.
- `{id?}`: tham số `id` không bắt buộc.

Ví dụ:

| URL | Controller/action |
|---|---|
| `/` | `HomeController.Index()` |
| `/SanPham` | `SanPhamController.Index()` |
| `/SanPham/Details/5` | `SanPhamController.Details(5)` |

Action có thể nhận giá trị từ các phần của URL:

```csharp
public IActionResult Details(int id)
{
    return Content($"Mã sản phẩm: {id}");
}
```

Tên tham số trong route thường được ghép theo tên tham số của action. Có thể dùng route constraint như `{id:int}` để chỉ khớp URL khi `id` là số nguyên.

## 2. Cơ chế định tuyến với `[Route]`

`[Route]` khai báo route theo attribute. Có thể đặt trên controller để làm tiền tố, rồi đặt route trên action để bổ sung phần còn lại:

```csharp
[Route("san-pham")]
public class SanPhamController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => View();

    [HttpGet("{id:int}/chi-tiet")]
    public IActionResult Details(int id) => View();
}
```

Trong ví dụ này, các URL là:

- `GET /san-pham` gọi `Index`.
- `GET /san-pham/5/chi-tiet` gọi `Details(5)`.
- `{id:int}` là constraint: URL chỉ khớp khi `id` là số nguyên.

Attribute routing thường đi cùng `[HttpGet]`, `[HttpPost]`, `[HttpPut]` hoặc `[HttpDelete]` để ràng buộc route theo HTTP method. Route bắt đầu bằng `/` hoặc `~/` trên action là route tuyệt đối, không ghép tiền tố của controller.

Controller demo chạy được trong dự án nằm tại [Controllers/RoutingDemoController.cs](../Controllers/RoutingDemoController.cs). Các URL mẫu:

| HTTP method | URL | Kết quả |
|---|---|---|
| `GET` | `/demo-routing` | Hiển thị endpoint demo |
| `GET` | `/demo-routing/san-pham/5` | Hiển thị mã sản phẩm `5` |
| `GET` | `/demo-routing/tim-kiem?keyword=banh` | Hiển thị từ khóa `banh` |

Controller dùng `Content(...)` để minh họa việc endpoint được chọn mà không cần tạo view. Route demo được khai báo bằng attribute; route mặc định trong `Program.cs` vẫn dùng cho các controller khác.

## 3. Lỗi thường gặp khi truy cập URL

### 404 Not Found

Không có endpoint phù hợp hoặc action không tìm thấy tài nguyên. Các nguyên nhân phổ biến:

- Sai tên hoặc cấu trúc URL.
- Action không có route phù hợp hoặc bị giới hạn bởi route constraint.
- Action gọi `NotFound()` do không tìm thấy dữ liệu.
- Đường dẫn đến view không đúng (trường hợp này có thể biểu hiện thành lỗi khi render view, không nhất thiết là lỗi route).

Ví dụ, `/demo-routing/san-pham/abc` không khớp `{id:int}`.

### 405 Method Not Allowed

Đường dẫn có thể khớp endpoint nhưng HTTP method không được hỗ trợ. Ví dụ, mở action chỉ nhận `POST` bằng trình duyệt thường sẽ gửi `GET`. Dùng `[HttpGet]` và `[HttpPost]` để khai báo rõ method được hỗ trợ.

### Lỗi bind hoặc validation

Giá trị không đúng kiểu, chẳng hạn chuỗi `abc` cho tham số `int`, có thể khiến model binding thất bại. Trong MVC thông thường, cần kiểm tra `ModelState.IsValid` và trả lại lỗi phù hợp; lỗi bind không tự động đồng nghĩa với HTTP 400. Với API controller có `[ApiController]`, ASP.NET Core thường tự trả 400 khi model state không hợp lệ.

### Không tìm thấy view

Routing có thể gọi action thành công nhưng MVC không tìm thấy file view. Với `return View()` trong `SanPhamController.Create`, vị trí mặc định là `Views/SanPham/Create.cshtml`. Kiểm tra tên thư mục, tên file và giá trị được truyền cho `View(...)`.

## 4. Cách kiểm tra khi route không hoạt động

1. So khớp URL với route mặc định hoặc các attribute `[Route]`.
2. Kiểm tra HTTP method của request với `[HttpGet]`, `[HttpPost]` hoặc attribute tương ứng.
3. Kiểm tra route constraint và giá trị tham số.
4. Đặt breakpoint trong action để xác định request có vào action hay không.
5. Nếu action đã chạy nhưng trang lỗi, kiểm tra việc truy vấn dữ liệu, `ModelState` và đường dẫn view.
