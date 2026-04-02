# QUY ĐỊNH KIỂM THỬ PHẦN MỀM - BACKEND

## 1. Mục tiêu
Đảm bảo chất lượng mã nguồn backend thông qua việc viết Unit Test cho các nghiệp vụ quan trọng, đặc biệt là các logic nghiệp vụ phức tạp trong tầng Service.

## 2. Phương pháp thiết kế Test Case
Sử dụng phương pháp **Đồ thị luồng điều khiển (Control Flow Graph - CFG)**.
- Vẽ hoặc xác định sơ đồ luồng dữ liệu cho từng phương thức cần kiểm thử.
- Xác định các điểm quyết định (Decision Points).
- Thiết kế các kịch bản kiểm thử bao quát toàn bộ các đường cơ bản (Basic Paths).

## 3. Tiêu chí độ bao phủ (Coverage)
Yêu cầu bắt buộc đạt được các mức độ bao phủ sau:

### 3.1. Độ bao phủ câu lệnh (Statement Coverage)
- **Mục tiêu**: Đạt 100% độ bao phủ câu lệnh cho các phương thức được chọn.
- Mỗi dòng mã trong logic nghiệp vụ phải được thực thi ít nhất một lần qua các bộ test case.

### 3.2. Độ bao phủ nhánh (Branch Coverage)
- **Mục tiêu**: Đạt 100% độ bao phủ nhánh.
- Mọi kết quả của các biểu thức điều kiện (True/False) phải được kiểm tra.
- Ví dụ: Với một câu lệnh `if`, phải có ít nhất một test case đi vào nhánh `then` và một test case đi vào nhánh `else`.

## 4. Công cụ sử dụng
- **Framework**: xUnit
- **Mocking**: Moq
- **Assertion**: FluentAssertions

## 5. Quy trình thực hiện
1. Phân tích mã nguồn phương thức mục tiêu.
2. Vẽ đồ thị CFG và xác định các nhánh.
3. Thiết kế bộ dữ liệu đầu vào (Input) và kết quả mong muốn (Expectation) cho từng trường hợp.
4. Triển khai mã kiểm thử.
5. Chạy kiểm thử và kiểm tra độ bao phủ (nếu có công cụ).

---
*Ghi chú: Luôn đảm bảo mock đầy đủ các Repository và UnitOfWork để Unit Test chạy độc lập với cơ sở dữ liệu.*
