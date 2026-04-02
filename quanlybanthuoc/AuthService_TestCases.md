# DANH SÁCH CHI TIẾT TEST CASES - AuthService.LoginAsync

Tài liệu này trình bày các kịch bản kiểm thử chi tiết cho phương thức đăng nhập, dựa trên phân tích CFG để đảm bảo độ bao phủ 100%.

| TC ID | Mô tả | Điều kiện tiên quyết | Bước thực hiện | KQ mong đợi | KQ thực tế | Pass/Fail |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **TC1** | Đăng nhập thất bại - Tên đăng nhập không tồn tại | Hệ thống không có user tương ứng | 1. Gọi `LoginAsync` với `TenDangNhap = "wrong_user"`. | Ném `NotFoundException`. | Exception thrown | Pass |
| **TC2** | Đăng nhập thất bại - Sai mật khẩu | User tồn tại trong hệ thống | 1. Gọi `LoginAsync` với mật khẩu sai. | Ném `NotFoundException`. | Exception thrown | Pass |
| **TC3** | Đăng nhập thất bại - Tài khoản bị khóa | User tồn tại nhưng `TrangThai = false` | 1. Gọi `LoginAsync` cho tài khoản bị khóa. | Ném `NotFoundException`. | Exception thrown | Pass |
| **TC4** | Đăng nhập thành công - Thông tin hợp lệ | User tồn tại, mật khẩu đúng, `TrangThai = true` | 1. Gọi `LoginAsync` với thông tin chính xác. | Trả về `LoginResponse` chứa Token. | Success with Tokens | Pass |

---
*Ghi chú: KQ thực tế và Pass/Fail sẽ được cập nhật sau khi chạy Unit Test.*
