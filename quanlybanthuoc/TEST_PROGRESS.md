# DANH SÁCH CHI TIẾT CÁC FILE VÀ PHƯƠNG THỨC KIỂM THỬ (UNIT TEST)

Dưới đây là danh sách toàn bộ các phương thức trong tầng Service cần được kiểm thử để đảm bảo độ bao phủ 100% Statement và Branch.

| STT | File Service | Phương Thức | Mức Độ Ưu Tiên | Trạng Thái |
| :--- | :--- | :--- | :--- | :--- |
| **1** | **DonHangService.cs** | `CreateAsync` | Cao | [x] Đã hoàn thành |
| 1.2 | | `CreateCustomerOrderAsync` | Cao | [x] Đã hoàn thành |
| 1.3 | | `GetByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 1.4 | | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 1.5 | | `DeleteAsync` | Trung bình | [ ] Chưa hoàn thành |
| 1.6 | | `UpdateAsync` | Trung bình | [ ] Chưa hoàn thành |
| **2** | **AuthService.cs** | `LoginAsync` | Cao | [x] Đã hoàn thành |
| 2.2 | | `LogoutAsync` | Trung bình | [ ] Chưa hoàn thành |
| 2.3 | | `RefreshTokenAsync` | Trung bình | [ ] Chưa hoàn thành |
| **3** | **CustomerAuthService.cs** | `LoginAsync` | Cao | [ ] Chưa hoàn thành |
| 3.2 | | `RegisterAsync` | Cao | [ ] Chưa hoàn thành |
| 3.3 | | `SendOtpAsync` | Trung bình | [ ] Chưa hoàn thành |
| 3.4 | | `VerifyOtpAsync` | Trung bình | [ ] Chưa hoàn thành |
| **4** | **DonNhapHangService.cs** | `CreateAsync` | Cao | [x] Đã hoàn thành |
| 4.2 | | `GetByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 4.3 | | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 4.4 | | `UpdateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 4.5 | | `DeleteAsync` | Trung bình | [ ] Chưa hoàn thành |
| **5** | **DonGiaoHangService.cs** | `CreateAsync` | Cao | [ ] Chưa hoàn thành |
| 5.2 | | `GetByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 5.3 | | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 5.4 | | `AssignDeliveryPersonAsync` | Trung bình | [ ] Chưa hoàn thành |
| 5.5 | | `UpdateStatusAsync` | Trung bình | [ ] Chưa hoàn thành |
| 5.6 | | `CancelAsync` | Trung bình | [ ] Chưa hoàn thành |
| 5.7 | | `GetByNguoiGiaoHangIdAsync` | Thấp | [ ] Chưa hoàn thành |
| **6** | **ThuocService.cs** | `CreateAsync` | Cao | [ ] Chưa hoàn thành |
| 6.2 | | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 6.3 | | `GetByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 6.4 | | `SoftDeleteAsync` | Thấp | [ ] Chưa hoàn thành |
| 6.5 | | `UpdateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 6.6 | | `GetThuocSapHetHanAsync` | Trung bình | [ ] Chưa hoàn thành |
| 6.7 | | `GetThuocTonKhoThapAsync` | Trung bình | [ ] Chưa hoàn thành |
| 6.8 | | `GetByChiNhanhIdAsync` | Thấp | [ ] Chưa hoàn thành |
| **7** | **KhachHangService.cs** | `CreateAsync` | Cao | [ ] Chưa hoàn thành |
| 7.2 | | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 7.3 | | `GetByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 7.4 | | `GetBySdtAsync` | Thấp | [ ] Chưa hoàn thành |
| 7.5 | | `SoftDeleteAsync` | Thấp | [ ] Chưa hoàn thành |
| 7.6 | | `UpdateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 7.7 | | `UpdateDiemTichLuyAsync` | Cao | [ ] Chưa hoàn thành |
| **8** | **KhoHangService.cs** | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 8.2 | | `GetByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 8.3 | | `GetTonKhoThapAsync` | Trung bình | [ ] Chưa hoàn thành |
| 8.4 | | `UpdateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 8.5 | | `CreateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 8.6 | | `DeleteAsync` | Trung bình | [ ] Chưa hoàn thành |
| **9** | **LoHangService.cs** | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 9.2 | | `GetByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 9.3 | | `GetByThuocIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 9.4 | | `GetLoHangSapHetHanAsync` | Trung bình | [ ] Chưa hoàn thành |
| 9.5 | | `UpdateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 9.6 | | `CreateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 9.7 | | `DeleteAsync` | Trung bình | [ ] Chưa hoàn thành |
| **10** | **MomoPaymentService.cs** | `CreatePaymentAsync` | Cao | [ ] Chưa hoàn thành |
| 10.2 | | `ProcessPaymentCallbackAsync` | Cao | [ ] Chưa hoàn thành |
| 10.3 | | `GetPaymentStatusAsync` | Trung bình | [ ] Chưa hoàn thành |
| **11** | **BaoCaoService.cs** | `GetBaoCaoDoanhThuTheoThangAsync` | Trung bình | [ ] Chưa hoàn thành |
| 11.2 | | `GetBaoCaoDoanhThuTheoNgayAsync` | Trung bình | [ ] Chưa hoàn thành |
| 11.3 | | `GetTopThuocBanChayAsync` | Trung bình | [ ] Chưa hoàn thành |
| 11.4 | | `GetThongKeDashboardAsync` | Trung bình | [ ] Chưa hoàn thành |
| 11.5 | | `GetBaoCaoTheoNhanVienAsync` | Trung bình | [ ] Chưa hoàn thành |
| **12** | **CustomerPageService.cs** | `GetFeaturedMedicinesAsync` | Thấp | [ ] Chưa hoàn thành |
| 12.2 | | `SearchMedicinesAsync` | Trung bình | [ ] Chưa hoàn thành |
| 12.3 | | `GetMedicineDetailAsync` | Thấp | [ ] Chưa hoàn thành |
| 12.4 | | `GetCategoriesAsync` | Thấp | [ ] Chưa hoàn thành |
| 12.5 | | `GetCustomerProfileAsync` | Thấp | [ ] Chưa hoàn thành |
| 12.6 | | `UpdateCustomerProfileAsync` | Trung bình | [ ] Chưa hoàn thành |
| 12.7 | | `GetOrderHistoryAsync` | Thấp | [ ] Chưa hoàn thành |
| 12.8 | | `GetOrderDetailAsync` | Thấp | [ ] Chưa hoàn thành |
| 12.9 | | `TrackShipmentAsync` | Trung bình | [ ] Chưa hoàn thành |
| 12.10 | | `GetLoyaltyPointHistoryAsync` | Thấp | [ ] Chưa hoàn thành |
| 12.11 | | `GetBranchesAsync` | Thấp | [ ] Chưa hoàn thành |
| 12.12 | | `GetStockByBranchAsync` | Thấp | [ ] Chưa hoàn thành |
| 12.13 | | `GetOrdersByPhoneAsync` | Trung bình | [ ] Chưa hoàn thành |
| **13** | **ChiNhanhService.cs** | `CreateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 13.2 | | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 13.3 | | `GetByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 13.4 | | `SoftDeleteAsync` | Thấp | [ ] Chưa hoàn thành |
| 13.5 | | `UpdateAsync` | Trung bình | [ ] Chưa hoàn thành |
| **14** | **DanhMucService.cs** | `CreateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 14.2 | | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 14.3 | | `GetByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 14.4 | | `UpdateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 14.5 | | `DeleteAsync` | Trung bình | [ ] Chưa hoàn thành |
| **15** | **NguoiDungService.cs** | `createAsync` | Trung bình | [ ] Chưa hoàn thành |
| 15.2 | | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 15.3 | | `getByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 15.4 | | `SoftDeleteAsync` | Thấp | [ ] Chưa hoàn thành |
| 15.5 | | `updateAsync` | Trung bình | [ ] Chưa hoàn thành |
| **16** | **NhaCungCapService.cs** | `CreateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 16.2 | | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 16.3 | | `GetByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 16.4 | | `SoftDeleteAsync` | Thấp | [ ] Chưa hoàn thành |
| 16.5 | | `UpdateAsync` | Trung bình | [ ] Chưa hoàn thành |
| **17** | **PhuongThucThanhToanService.cs** | `CreateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 17.2 | | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 17.3 | | `GetByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 17.4 | | `UpdateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 17.5 | | `DeleteAsync` | Trung bình | [ ] Chưa hoàn thành |
| **18** | **TokenService.cs** | `GenerateAccessToken` | Cao | [ ] Chưa hoàn thành |
| 18.2 | | `GenerateRefreshToken` | Cao | [ ] Chưa hoàn thành |
| **19** | **VaiTroService.cs** | `CreateAsync` | Trung bình | [ ] Chưa hoàn thành |
| 19.2 | | `GetAllAsync` | Thấp | [ ] Chưa hoàn thành |
| 19.3 | | `GetAllActiveAsync` | Thấp | [ ] Chưa hoàn thành |
| 19.4 | | `GetByIdAsync` | Thấp | [ ] Chưa hoàn thành |
| 19.5 | | `SoftDeleteAsync` | Trung bình | [ ] Chưa hoàn thành |
| 19.6 | | `UpdateAsync` | Trung bình | [ ] Chưa hoàn thành |

---
*Ghi chú: Đánh dấu [x] sau khi hoàn thành.*
