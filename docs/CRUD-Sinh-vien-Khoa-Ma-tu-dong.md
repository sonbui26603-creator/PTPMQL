# CRUD Sinh viên – Khoa và mã sinh viên tự động

## 1. Mô hình dữ liệu và khóa ngoại

- `Khoa` là bảng khoa, khóa chính `MaKhoa`.
- `SinhVien` là bảng sinh viên, khóa chính là `MaSinhVien`; cột `MaKhoa` là khóa ngoại tham chiếu `Khoa.MaKhoa`.
- `SinhVienCodeSequence` lưu số thứ tự cuối theo từng tổ hợp năm nhập học, mã hệ đào tạo và mã ngành.
- Xóa khoa đang có sinh viên bị từ chối bởi ràng buộc `Restrict`, tránh xóa mất dữ liệu liên quan.

Danh sách khoa khởi tạo trong `AppDbContext` là **dữ liệu mẫu**, với mã khoa minh họa để chạy chức năng và có thể chỉnh sửa theo danh mục chính thức của trường. Khi sửa dữ liệu `HasData`, tạo migration mới rồi cập nhật database.

## 2. Cấu trúc mã sinh viên

Mã có 10 chữ số:

```text
YY + HH + NN + SSSS
```

- `YY`: hai số cuối của năm nhập học. Ví dụ 2028 → `28`.
- `HH`: mã hệ đào tạo, đúng 2 chữ số.
- `NN`: mã ngành, đúng 2 chữ số.
- `SSSS`: số thứ tự 4 chữ số, bắt đầu từ `0001` cho mỗi tổ hợp năm/hệ/ngành.

Ví dụ mã `2821050199` được tách thành năm `28`, hệ `21`, ngành `05`, thứ tự `0199`. Sinh viên kế tiếp cùng tổ hợp nhận mã `2821050200`. Nếu đổi năm, hệ hoặc ngành, bộ đếm là bộ riêng và bắt đầu từ `0001`.

Mã hệ và mã ngành được nhập dưới dạng chuỗi để giữ số 0 ở đầu (`05`). Mã sinh viên không được nhập từ form; ứng dụng sinh và lưu tự động.

## 3. Tính duy nhất khi tạo đồng thời

Việc tạo sinh viên, tăng bộ đếm và lưu mã được thực hiện trong cùng transaction. SQL Server `sp_getapplock` khóa riêng theo tổ hợp năm/hệ/ngành cho đến khi transaction kết thúc. Nhờ vậy, hai request cùng tạo sinh viên trong một tổ hợp sẽ không lấy cùng số; các tổ hợp khác nhau vẫn có thể xử lý độc lập. Nếu lưu sinh viên thất bại, transaction rollback cả bộ đếm.

Trước khi cấp mã, ứng dụng đối chiếu mã lớn nhất đang có trong bảng sinh viên với bộ đếm. Vì vậy, nếu dữ liệu đã có `2821050199` mà chưa có dòng bộ đếm tương ứng, lần tạo tiếp theo vẫn nhận `2821050200`. Dữ liệu nhập ngoài ứng dụng cần tuân theo định dạng 10 chữ số.

Số thứ tự giới hạn từ `0001` đến `9999` cho mỗi tổ hợp. Khi hết dải, form hiển thị lỗi thay vì sinh mã sai độ dài.

## 4. CRUD và chạy migration

Giao diện CRUD nằm tại `/SinhVien`:

- Danh sách kèm tên khoa.
- Tạo sinh viên với dropdown chọn khoa; mã được sinh khi lưu.
- Xem chi tiết, sửa họ tên/khoa, và xác nhận xóa.
- Mã sinh viên giữ nguyên khi sửa thông tin.

Từ thư mục chứa file project, tạo migration và cập nhật SQL Server:

```powershell
dotnet ef migrations add AddStudentFacultyAndCodeSequence
dotnet ef database update
```

Sau khi cập nhật database, các khoa mẫu được seed và có thể chọn khi thêm sinh viên. Để xác nhận chuỗi mẫu, tạo sinh viên năm 2028, mã hệ `21`, mã ngành `05`: các mã đầu tiên sẽ là `2821050001`, `2821050002`, ... Mã `2821050199` sẽ được nối tiếp bằng `2821050200`.

Connection string cần được cấu hình an toàn qua User Secrets hoặc biến môi trường như hướng dẫn tại [Ket-noi-CSDL-va-CRUD.md](./Ket-noi-CSDL-va-CRUD.md); không commit thông tin đăng nhập thật.
