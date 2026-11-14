# Layout, Bootstrap 5 và CSS riêng trong ASP.NET Core MVC

## 1. Layout là gì?

Layout là khung giao diện dùng chung cho nhiều trang, thường chứa phần `<head>`, thanh điều hướng, vùng hiển thị nội dung và chân trang. Trong dự án này:

- `Views/Shared/_Layout.cshtml` định nghĩa khung trang.
- `Views/_ViewStart.cshtml` chọn `_Layout` làm layout mặc định cho các view.
- `@RenderBody()` là vị trí nội dung của từng view được chèn vào layout.
- `@RenderSectionAsync("Scripts", required: false)` cho phép view thêm script riêng khi cần, chẳng hạn script kiểm tra form.

Vì layout nằm trong `Views/Shared`, các view như `Views/Home/Index.cshtml` và `Views/SanPham/Index.cshtml` có thể dùng chung giao diện.

## 2. Bootstrap 5 trong dự án

Dự án đã có Bootstrap **5.3.3** trong `wwwroot/lib/bootstrap` cùng với CSS, JavaScript bundle và các file thư viện phụ thuộc. Vì vậy không cần cài lại Bootstrap hoặc tải thêm bản từ CDN.

Trong `Views/Shared/_Layout.cshtml`, Bootstrap CSS được nạp trong phần `<head>`:

```cshtml
<link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
```

Thứ tự quan trọng: Bootstrap được nạp trước, CSS riêng được nạp sau để quy tắc trong `site.css` có thể tùy biến giao diện Bootstrap. `asp-append-version="true"` thêm phiên bản theo nội dung file vào URL để trình duyệt nhận CSS mới sau khi cập nhật.

JavaScript bundle của Bootstrap được đặt gần cuối `<body>`:

```cshtml
<script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
```

Bundle bao gồm Popper và hỗ trợ các tương tác như menu navbar thu gọn trên màn hình nhỏ. Bootstrap 5 không cần jQuery; dự án vẫn tải jQuery riêng vì các script kiểm tra validation hiện dùng nó.

## 3. Tách CSS riêng và tích hợp vào Layout

Không nên đặt toàn bộ kiểu dáng riêng trong các thẻ `<style>` của từng view. Hãy lưu quy tắc dùng chung tại `wwwroot/css/site.css`, rồi tham chiếu file từ Layout:

```cshtml
<link rel="stylesheet" href="~/css/site.css" asp-append-version="true" />
```

Ví dụ trong `wwwroot/css/site.css`:

```css
.app-header .navbar-brand {
  font-weight: 700;
}

.app-header .nav-link:hover {
  color: var(--bs-primary);
}
```

Các selector có tiền tố riêng như `.app-header` giúp giới hạn phạm vi tùy biến và tránh ảnh hưởng không mong muốn đến các thành phần khác.

## 4. Luồng render trang

1. Người dùng truy cập một action, ví dụ `SanPham/Index`.
2. Action trả về View.
3. MVC render nội dung view tại `@RenderBody()` trong `_Layout.cshtml`.
4. Trình duyệt tải Bootstrap CSS, `site.css`, rồi hiển thị trang với các quy tắc được áp dụng theo thứ tự.
5. Bootstrap bundle xử lý các tương tác giao diện như nút mở/đóng navbar.

Layout hiện đặt `lang="vi"`, có thanh điều hướng đến trang chủ và danh sách sản phẩm, cùng chân trang. Các kiểu dáng riêng của header, nội dung và footer nằm trong `wwwroot/css/site.css`.
