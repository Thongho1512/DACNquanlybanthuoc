# Bộ Test Case Cho Phương Thức CreateAsync (Tạo Lô Hàng Mới) - LoHangService

Tài liệu này chi tiết các test case dựa trên phân tích độ bao phủ câu lệnh (Statement Coverage) cho phương thức `CreateAsync` trong `LoHangService`.

## Phân Tích Logic
Phương thức `CreateAsync` thực hiện các bước:
1. Kiểm tra sự tồn tại và trạng thái của Chi nhánh.
2. Kiểm tra sự tồn tại và trạng thái của Thuốc.
3. Kiểm tra tính hợp lệ của Ngày sản xuất và Ngày hết hạn.
4. Bắt đầu Transaction.
5. Tạo bản ghi Lô hàng (`LoHang`).
6. Kiểm tra bản ghi Kho hàng (`KhoHang`) tương ứng:
   - Nếu chưa có: Tạo mới bản ghi Kho hàng.
   - Nếu đã có: Cộng dồn số lượng tồn kho.
7. Lưu thay đổi và Commit Transaction.

---

## Bảng Test Case Chi Tiết

| TC ID | Mô tả | Điều kiện tiên quyết | Bước thực hiện | KQ mong đợi | KQ thực tế | Pass/Fail |
|-------|-------|----------------------|----------------|-------------|------------|-----------|
| **TC1** | Tạo lô hàng với Chi nhánh không tồn tại | - Database không có Chi nhánh ID = 0 hoặc Chi nhánh ID = 0 có TrangThai = False. | Gọi `CreateAsync(dto, idChiNhanh: 0)` | Hệ thống ném ra lỗi `NotFoundException` với thông báo "Chi nhánh không tồn tại hoặc không hoạt động." | | |
| **TC2** | Tạo lô hàng với Thuốc không tồn tại | - Chi nhánh hợp lệ.<br>- Database không có Thuốc ID = 999. | Gọi `CreateAsync(dto: {Idthuoc: 999}, idChiNhanh: 1)` | Hệ thống ném ra lỗi `NotFoundException` với thông báo "Thuốc không tồn tại." | | |
| **TC3** | Ngày hết hạn không hợp lệ (trước hoặc bằng ngày sản xuất) | - Chi nhánh và Thuốc đều hợp lệ. | Gọi `CreateAsync` với:<br>- NgaySanXuat: 2025-06-01<br>- NgayHetHan: 2025-01-01 | Hệ thống ném ra lỗi `BadRequestException` với thông báo "Ngày hết hạn phải sau ngày sản xuất." | | |
| **TC4** | Ngày hết hạn đã qua (thuốc đã hết hạn) | - Chi nhánh và Thuốc đều hợp lệ.<br>- Ngày hiện tại là 2024-03-30. | Gọi `CreateAsync` với:<br>- NgayHetHan: 2024-01-01 | Hệ thống ném ra lỗi `BadRequestException` với thông báo "Không thể nhập thuốc đã hết hạn." | | |
| **TC5** | Tạo lô hàng thành công (Kho hàng chưa tồn tại cho lô này) | - Mọi thông tin hợp lệ.<br>- Kho hàng cho cặp (Chi nhánh, Lô hàng mới) chưa có trong DB. | 1. Gọi `CreateAsync` với các tham số hợp lệ.<br>2. Kiểm tra database bản ghi LoHang và KhoHang. | 1. Trả về `LoHangDto` đầy đủ thông tin.<br>2. Bản ghi `LoHang` mới được tạo.<br>3. Bản ghi `KhoHang` mới được tạo với `SoLuongTon = dto.SoLuong`. | | |
| **TC6** | Tạo lô hàng thành công (Kho hàng đã tồn tại cho lô này) | - Mọi thông tin hợp lệ.<br>- Giả lập trường hợp Kho hàng cho lô này đã tồn tại (nếu có). | 1. Gọi `CreateAsync` với các tham số hợp lệ.<br>2. Kiểm tra database. | 1. Trả về `LoHangDto`.<br>2. Bản ghi `LoHang` mới được tạo.<br>3. Bản ghi `KhoHang` hiện tại được cộng thêm `dto.SoLuong`. | | |
| **TC7** | Lỗi hệ thống trong quá trình lưu dữ liệu (Transaction Rollback) | - Mọi thông tin hợp lệ.<br>- Giả lập lỗi khi lưu `LoHangRepository.CreateAsync` hoặc `SaveChangesAsync`. | Gọi `CreateAsync` và kích hoạt lỗi giả lập. | 1. Exception được throw.<br>2. Transaction được Rollback (không có dữ liệu rác trong DB).<br>3. Log lỗi được ghi lại. | | |

---

## Ghi chú cho Unit Test
- Sử dụng **Moq** để giả lập các Repository (`IUnitOfWork`, `ILoHangRepository`, `IChiNhanhRepository`, etc.).
- Sử dụng **FluentAssertions** để kiểm tra kết quả (trường hợp Exception và trường hợp thành công).
- Đối với **TC6**, trong ngữ cảnh thực tế của phương thức `CreateAsync` thủ công, việc `khoHang` đã tồn tại cho một `loHang.Id` vừa mới sinh ra là rất hiếm, nhưng logic code vẫn xử lý nhánh này nên cần được kiểm tra để đạt độ bao phủ 100%.
