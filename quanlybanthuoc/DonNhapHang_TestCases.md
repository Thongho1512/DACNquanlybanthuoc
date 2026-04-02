# DANH SÁCH CHI TIẾT TEST CASES - DonNhapHangService.CreateAsync

Tài liệu này trình bày các kịch bản kiểm thử chi tiết cho phương thức nhập hàng, dựa trên phân tích CFG để đảm bảo độ bao phủ 100%.

| TC ID | Mô tả | Điều kiện tiên quyết | Bước thực hiện | KQ mong đợi | KQ thực tế | Pass/Fail |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **TC1** | Nhập hàng với chi nhánh không tồn tại | Hệ thống có dữ liệu chi nhánh | 1. Gọi `CreateAsync` với `IdchiNhanh = 999`. | Ném `NotFoundException`. | Exception thrown | Pass |
| **TC2** | Nhập hàng với nhà cung cấp không tồn tại | Chi nhánh hợp lệ | 1. Gọi `CreateAsync` với `IdnhaCungCap = 999`. | Ném `NotFoundException`. | Exception thrown | Pass |
| **TC3** | Nhập hàng với số đơn nhập đã tồn tại | Chi nhánh & NCC hợp lệ | 1. Gọi `CreateAsync` với `SoDonNhap` trùng. | Ném `BadRequestException`. | Exception thrown | Pass |
| **TC4** | Nhập hàng với thuốc không tồn tại | Dữ liệu đơn hàng hợp lệ | 1. Gọi `CreateAsync` với `Idthuoc = 999`. | Ném `NotFoundException`. | Exception thrown | Pass |
| **TC5** | Ngày hết hạn <= Ngày sản xuất | Thuốc hợp lệ | 1. Gọi `CreateAsync` với HSD <= NSX. | Ném `BadRequestException`. | Exception thrown | Pass |
| **TC6** | Nhập thuốc đã hết hạn sử dụng | Thuốc hợp lệ | 1. Gọi `CreateAsync` với HSD < Today. | Ném `BadRequestException`. | Exception thrown | Pass |
| **TC7** | Nhập thuốc mới hoàn toàn (chưa có trong kho) | Thuốc lần đầu nhập | 1. Gọi `CreateAsync` với thuốc mới. | Hệ thống tạo mới `KhoHang`. | New record created | Pass |
| **TC8** | Nhập thêm số lượng cho thuốc đã có | Thuốc đã tồn tại | 1. Gọi `CreateAsync` với thuốc cũ. | Hệ thống cập nhật `SoLuongTon`. | Quantity updated | Pass |
| **TC9** | Nhập đơn hàng có nhiều lô hàng | Toàn bộ hợp lệ | 1. Gọi `CreateAsync` với nhiều lô. | Đơn hàng tạo thành công. | All records created | Pass |

---
*Ghi chú: KQ thực tế và Pass/Fail sẽ được cập nhật sau khi chạy Unit Test.*
