# PHÂN TÍCH CFG VÀ THIẾT KẾ TEST CASE - DonNhapHangService.CreateAsync

Tài liệu này trình bày quy trình kiểm thử cho phương thức nhập hàng (`CreateAsync`) thuộc `DonNhapHangService`.

---

## 1. Sơ đồ luồng điều khiển (CFG)

```mermaid
graph TD
    N1([Entry]) --> N2{D1: Branch Valid?}
    N2 -- No --> N3[Throw NotFound]
    N3 --> EXIT_E([Exit Error])
    
    N2 -- Yes --> N4{D2: Supplier Valid?}
    N4 -- No --> N5[Throw NotFound]
    N5 --> EXIT_E
    
    N4 -- Yes --> N6{D3: Duplicate Code?}
    N6 -- Yes --> N7[Throw BadRequest]
    N7 --> EXIT_E
    
    N6 -- No --> N8[Begin Transaction & Create Order]
    N8 --> N9{Loop: Batches}
    
    N9 -- Item --> N10[Get Medicine]
    N10 --> N11{D4: Med Valid?}
    N11 -- No --> N12[Throw NotFound]
    N12 -.-> EX_CATCH
    
    N11 -- Yes --> N13{D5: Exp <= Prod?}
    N13 -- Yes --> N14[Throw BadRequest]
    N14 -.-> EX_CATCH
    
    N13 -- No --> N15{D6: Exp <= Now?}
    N15 -- Yes --> N16[Throw BadRequest]
    N16 -.-> EX_CATCH
    
    N15 -- No --> N17[Calc Price & Create Batch]
    N17 --> N18[Check Warehouse]
    N18 --> N19{D7: Stock Exists?}
    N19 -- No --> N20[Create New Stock]
    N20 --> N22[Add to List]
    N19 -- Yes --> N21[Update Stock]
    N21 --> N22
    N22 --> N9
    
    N9 -- Done --> N23[Finalize Totals & Commit]
    N23 --> N24[Return Result]
    N24 --> EXIT_S([Exit Success])
    
    subgraph Exception_Handling
    EX_CATCH[Catch Block] --> ROLLBACK[Rollback]
    ROLLBACK --> EXIT_E
    end
```

---

## 2. Thiết kế Test Cases (CFG Based)

Dựa trên sơ đồ CFG, chúng ta xác định **9 kịch bản** chính để bao phủ 100% các nhánh (D1 - D7) và các vòng lặp.

| STT | Test Case ID | Path | Mô tả kịch bản | Kết quả mong đợi |
| :--- | :--- | :--- | :--- | :--- |
| 1 | **TC1** | N2(No) → N3 | Chi nhánh không tồn tại hoặc bị khóa. | `NotFoundException` |
| 2 | **TC2** | N4(No) → N5 | Nhà cung cấp không tồn tại hoặc bị khóa. | `NotFoundException` |
| 3 | **TC3** | N6(Yes) → N7 | Số hóa đơn nhập hàng bị trùng lặp. | `BadRequestException` |
| 4 | **TC4** | N11(No) → N12 | Thuốc trong danh sách nhập không tồn tại. | `NotFoundException` |
| 5 | **TC5** | N13(Yes) → N14 | Ngày hết hạn nhỏ hơn ngày sản xuất. | `BadRequestException` |
| 6 | **TC6** | N15(Yes) → N16 | Thuốc nhập vào đã quá hạn sử dụng. | `BadRequestException` |
| 7 | **TC7** | N19(No) → N20 | Nhập thuốc mới hoàn toàn (chưa có trong kho). | Tạo mới `KhoHang` |
| 8 | **TC8** | N19(Yes) → N21 | Nhập thêm số lượng cho thuốc đã có. | Cập nhật `KhoHang` |
| 9 | **TC9** | Loop Multi | Nhập đơn hàng có nhiều lô hàng khác nhau. | Thành công, tổng tiền đúng. |

---

## 3. Xác nhận Độ Bao Phủ

- **Cyclomatic Complexity (McCabe)**: **10** (V(G) = 7 Decisions + 1 + 2 = 10 đường dẫn cơ bản).
- **Statement Coverage**: Đạt **100%** (Tất cả các lệnh Save, DB Context được thực thi).
- **Branch Coverage**: Đạt **100%** (Cả hai nhánh True/False của validate và loop đều được bao phủ).

---
*TÀI LIỆU KIỂM THỬ PHẦN MỀM LƯU HÀNH NỘI BỘ*
