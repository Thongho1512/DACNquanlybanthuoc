# PHÂN TÍCH CONTROL FLOW GRAPH (CFG) - DonNhapHangService.CreateAsync

Tài liệu này trình bày phân tích các nút quyết định và xác định các đường đi độc lập cho phương thức `CreateAsync` trong `DonNhapHangService.cs`.

## 1. Danh sách các nút quyết định (Predicate Nodes)

| ID | Dòng code | Điều kiện (Predicate) | Kết quả |
|:---|:---|:---|:---|
| **P1** | 32 | `chiNhanh == null \|\| chiNhanh.TrangThai != true` | Throw NotFound |
| **P2** | 39 | `nhaCungCap == null \|\| nhaCungCap.TrangThai != true` | Throw NotFound |
| **P3** | 46 | `existing != null` (SoDonNhap) | Throw BadRequest |
| **P4** | 72 | `foreach (var loHangDto in dto.LoHangs)` | Vòng lặp các lô hàng nhập |
| **P5** | 76 | `thuoc == null \|\| thuoc.TrangThai != true` | Throw NotFound |
| **P6** | 82 | `loHangDto.NgayHetHan <= loHangDto.NgaySanXuat` | Throw BadRequest |
| **P7** | 87 | `loHangDto.NgayHetHan <= DateOnly.FromDateTime(DateTime.Now)` | Throw BadRequest |
| **P8** | 114 | `if (khoHang == null)` | Tạo mới hoặc Cập nhật kho |
| **P9** | 150 | `catch (Exception ex)` | Rollback & Throw |

## 2. Tính toán Độ phức tạp Cyclomatic $V(G)$

$V(G) = P + 1 = 9 + 1 = 10$.

Cần ít nhất **10 đường đi độc lập** để bao phủ hoàn toàn các logic rẽ nhánh.

## 3. Xác định Independent Paths (Sơ bộ)

1. **Path 1**: P1(True) -> Exit (Branch Not Found).
2. **Path 2**: P1(False) -> P2(True) -> Exit (Supplier Not Found).
3. **Path 3**: P1(False) -> P2(False) -> P3(True) -> Exit (Duplicate Invoice No).
4. **Path 4**: P3(False) -> P4(Empty List) -> End Success.
5. **Path 5**: P4(Has item) -> P5(True) -> Exit (Medicine Not Found).
6. **Path 6**: P4(Has item) -> P5(False) -> P6(True) -> Exit (Invalid Dates).
7. **Path 7**: P4(Has item) -> P6(False) -> P7(True) -> Exit (Expired Product).
8. **Path 9**: P7(False) -> P8(True) -> Tạo mới KhoHang -> Continue Loop -> End.
9. **Path 10**: P7(False) -> P8(False) -> Cập nhật KhoHang -> Continue Loop -> End.
10. **Path 11**: Try block -> Runtime error -> P9(Catch) -> Rollback.

## 4. Danh sách Test Case thiết kế

| TC ID | Mô tả | Đầu vào mẫu | Kết quả mong đợi |
|:---|:---|:---|:---|
| **TC01** | Chi nhánh không hợp lệ | `IdchiNhanh = 999` | `NotFoundException` |
| **TC02** | Nhà cung cấp không hợp lệ | `IdnhaCungCap = 999` | `NotFoundException` |
| **TC03** | Số đơn nhập đã tồn tại | `SoDonNhap = "HD001"` (có sẵn) | `BadRequestException` |
| **TC04** | Thuốc nhập không tồn tại | `Idthuoc = 999` | `NotFoundException` |
| **TC05** | HSD trước NSX | `NSX=2024, HSD=2023` | `BadRequestException` |
| **TC06** | Thuốc đã hết hạn | `HSD=2020` | `BadRequestException` |
| **TC07** | Nhập thành công (Lô mới chưa có trong kho) | Thuốc A, Lô X1 (mới) | `KhoHangRepository.CreateAsync` được gọi |
| **TC08** | Nhập thành công (Cập nhật lô đã có trong kho) | Thuốc A, Lô X1 (đã có) | `KhoHangRepository.UpdateAsync` được gọi |
| **TC09** | Nhập nhiều lô hàng cùng lúc | List 3 lô | `LoHangRepository.CreateAsync` gọi 3 lần |
| **TC10** | Lỗi khi lưu dữ liệu (Rollback) | Exception tại SaveChanges | `RollbackTransactionAsync` được gọi |
