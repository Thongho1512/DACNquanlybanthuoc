# Hướng Dẫn Tích Hợp Thanh Toán Momo

## 📋 Mục Lục
1. [Đăng Ký Momo Sandbox](#đăng-ký-momo-sandbox)
2. [Cấu Hình Backend](#cấu-hình-backend)
3. [Tạo Migration Database](#tạo-migration-database)
4. [Luồng Thanh Toán](#luồng-thanh-toán)
5. [API Endpoints](#api-endpoints)
6. [Testing](#testing)

---

## 🔐 Đăng Ký Momo Sandbox

### Bước 1: Đăng ký tài khoản Developer
1. Truy cập: https://developers.momo.vn/
2. Đăng ký tài khoản developer (nếu chưa có)
3. Đăng nhập vào Developer Portal

### Bước 2: Tạo ứng dụng mới
1. Vào **"Ứng dụng"** → **"Tạo ứng dụng mới"**
2. Điền thông tin:
   - **Tên ứng dụng**: QuanLyBanThuoc (hoặc tên bạn muốn)
   - **Mô tả**: Ứng dụng quản lý bán thuốc
   - **Loại ứng dụng**: Chọn **"Thanh toán"**
   - **Môi trường**: Chọn **"Sandbox"** (để test)

### Bước 3: Lấy thông tin API
Sau khi tạo ứng dụng, bạn sẽ nhận được:
- **Partner Code**: Mã đối tác
- **Access Key**: Khóa truy cập
- **Secret Key**: Khóa bí mật (quan trọng, giữ bí mật!)

### Bước 4: Cấu hình Webhook/Notify URL
1. Vào **"Cấu hình"** → **"IPN URL"** (Instant Payment Notification)
2. Nhập URL webhook của bạn:
   ```
   https://your-domain.com/api/v1/payments/notify
   ```
   - **Lưu ý**: URL phải là HTTPS và có thể truy cập công khai từ internet
   - Nếu test local, có thể dùng ngrok để expose localhost:
     ```bash
     ngrok http 5000
     ```
     Sau đó dùng URL ngrok: `https://xxxx.ngrok.io/api/v1/payments/notify`

3. **Return URL** (URL redirect sau khi thanh toán):
   ```
   https://your-domain.com/payment/return
   ```

---

## ⚙️ Cấu Hình Backend

### Bước 1: Cập nhật appsettings.json

Mở file `appsettings.json` và cập nhật thông tin Momo:

```json
{
  "Momo": {
    "ApiEndpoint": "https://test-payment.momo.vn/v2/gateway/api/create",
    "PartnerCode": "YOUR_PARTNER_CODE",
    "AccessKey": "YOUR_ACCESS_KEY",
    "SecretKey": "YOUR_SECRET_KEY",
    "ReturnUrl": "https://your-domain.com/payment/return",
    "NotifyUrl": "https://your-domain.com/api/v1/payments/notify"
  }
}
```

**Lưu ý quan trọng:**
- **Sandbox**: Dùng `https://test-payment.momo.vn/v2/gateway/api/create`
- **Production**: Dùng `https://payment.momo.vn/v2/gateway/api/create` (sau khi được approve)
- Thay `YOUR_PARTNER_CODE`, `YOUR_ACCESS_KEY`, `YOUR_SECRET_KEY` bằng giá trị thực từ Momo Developer Portal
- Thay `your-domain.com` bằng domain thực của bạn

### Bước 2: Kiểm tra các service đã được đăng ký

Đảm bảo trong `Program.cs` đã có:
```csharp
builder.Services.AddHttpClient();
builder.Services.AddScoped<IPaymentService, MomoPaymentService>();
```

---

## 🗄️ Tạo Migration Database

### Bước 1: Tạo migration
Mở Terminal/PowerShell tại thư mục project và chạy:

```bash
dotnet ef migrations add AddPaymentFieldsToDonHang
```

### Bước 2: Kiểm tra migration
Kiểm tra file migration được tạo trong thư mục `Migrations/` để đảm bảo có các field:
- `TrangThaiThanhToan` (string, max 50, default: "PENDING_PAYMENT")
- `MomoOrderId` (string, max 100, nullable)
- `MomoTransactionId` (string, max 100, nullable)
- `NgayThanhToan` (datetime, nullable)

### Bước 3: Áp dụng migration
```bash
dotnet ef database update
```

---

## 🔄 Luồng Thanh Toán

### Luồng tổng quan:

```
1. Frontend tạo đơn hàng
   POST /api/v1/donhangs
   → Backend tạo đơn với TrangThaiThanhToan = "PENDING_PAYMENT"

2. Frontend gọi API tạo payment
   POST /api/v1/payments/create
   Body: { orderId, amount, paymentMethodCode: "MOMO" }
   → Backend gọi Momo API → Nhận về paymentUrl và qrCodeUrl

3. Frontend hiển thị QR Code
   → User quét QR và thanh toán trên app Momo

4. Momo gọi webhook
   POST /api/v1/payments/notify
   → Backend verify signature và cập nhật:
     - TrangThaiThanhToan = "PAID"
     - MomoTransactionId
     - NgayThanhToan
     - Tạo DonGiaoHang nếu LoaiDonHang = "GIAO_HANG"

5. Frontend poll status hoặc nhận redirect
   GET /api/v1/payments/{orderId}/status
   → Kiểm tra TrangThaiThanhToan
```

### Các trạng thái thanh toán:

- **PENDING_PAYMENT**: Đang chờ thanh toán (Momo)
- **PAID**: Đã thanh toán thành công
- **PAID_ON_DELIVERY**: Thanh toán khi nhận hàng (tiền mặt)
- **FAILED**: Thanh toán thất bại
- **CANCELLED**: Đã hủy

---

## 📡 API Endpoints

### 1. Tạo Payment Request (QR Code)

**Endpoint:** `POST /api/v1/payments/create`

**Headers:**
```
Authorization: Bearer {token}
Content-Type: application/json
```

**Request Body:**
```json
{
  "orderId": 123,
  "amount": 50000,
  "paymentMethodCode": "MOMO",
  "returnUrl": "https://your-domain.com/payment/return",
  "notifyUrl": "https://your-domain.com/api/v1/payments/notify"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Thành công.",
  "data": {
    "paymentUrl": "https://test-payment.momo.vn/...",
    "qrCodeUrl": "https://test-payment.momo.vn/qr/...",
    "orderId": "ORDER_123_1234567890",
    "deepLink": "momo://..."
  }
}
```

**Frontend sử dụng:**
- `qrCodeUrl`: Hiển thị QR Code để user quét
- `paymentUrl`: Redirect user đến trang thanh toán Momo
- `deepLink`: Mở app Momo trực tiếp (nếu có)

### 2. Webhook Callback (Momo gọi)

**Endpoint:** `POST /api/v1/payments/notify`

**Lưu ý:** Endpoint này **AllowAnonymous** vì Momo gọi từ bên ngoài.

**Request Body (từ Momo):**
```json
{
  "partnerCode": "MOMO",
  "orderId": "ORDER_123_1234567890",
  "requestId": "1234567890",
  "amount": 50000,
  "orderInfo": "Thanh toan don hang #123",
  "orderType": "momo_wallet",
  "transId": "1234567890",
  "resultCode": 0,
  "message": "Success",
  "payType": "web",
  "responseTime": 1234567890,
  "extraData": "",
  "signature": "abc123..."
}
```

**Response (cho Momo):**
```json
{
  "resultCode": 0,
  "message": "Success"
}
```

### 3. Kiểm tra trạng thái thanh toán

**Endpoint:** `GET /api/v1/payments/{orderId}/status`

**Headers:**
```
Authorization: Bearer {token}
```

**Response:**
```json
{
  "success": true,
  "message": "Thành công.",
  "data": {
    "orderId": 123,
    "paymentStatus": "PAID",
    "momoOrderId": "ORDER_123_1234567890",
    "momoTransactionId": "1234567890",
    "ngayThanhToan": "2024-01-15T10:30:00",
    "message": null
  }
}
```

---

## 🧪 Testing

### Test với Momo Sandbox

1. **Tạo đơn hàng:**
   ```bash
   POST /api/v1/donhangs
   {
     "idkhachHang": 1,
     "idchiNhanh": 1,
     "idphuongThucTt": 2,  # ID của Momo
     "loaiDonHang": "GIAO_HANG",
     "chiTietDonHangs": [...]
   }
   ```

2. **Tạo payment request:**
   ```bash
   POST /api/v1/payments/create
   {
     "orderId": 123,
     "amount": 50000,
     "paymentMethodCode": "MOMO"
   }
   ```

3. **Lấy QR Code:**
   - Sử dụng `qrCodeUrl` từ response
   - Hiển thị QR Code trên frontend
   - Quét bằng app Momo (sandbox)

4. **Thanh toán test:**
   - Mở app Momo sandbox
   - Quét QR Code
   - Sử dụng tài khoản test để thanh toán
   - Momo sẽ gọi webhook về server

5. **Kiểm tra kết quả:**
   ```bash
   GET /api/v1/payments/123/status
   ```

### Test Webhook Local với ngrok

1. **Cài đặt ngrok:**
   ```bash
   # Download từ https://ngrok.com/
   # Hoặc dùng package manager
   ```

2. **Chạy ngrok:**
   ```bash
   ngrok http 5000
   ```

3. **Cập nhật NotifyUrl trong appsettings.json:**
   ```json
   "NotifyUrl": "https://xxxx.ngrok.io/api/v1/payments/notify"
   ```

4. **Cập nhật trong Momo Developer Portal:**
   - Vào cấu hình ứng dụng
   - Cập nhật IPN URL = `https://xxxx.ngrok.io/api/v1/payments/notify`

---

## 🔒 Bảo Mật

### 1. Verify Signature
- Backend luôn verify signature từ Momo để đảm bảo request hợp lệ
- Signature được tính bằng HMAC-SHA256

### 2. HTTPS Required
- Webhook URL phải là HTTPS
- Momo chỉ gọi webhook qua HTTPS

### 3. Secret Key
- **KHÔNG BAO GIỜ** commit Secret Key lên Git
- Sử dụng User Secrets hoặc Environment Variables:
  ```bash
  dotnet user-secrets set "Momo:SecretKey" "your-secret-key"
  ```

---

## 📝 Ghi Chú Quan Trọng

1. **Sandbox vs Production:**
   - Sandbox: Dùng để test, không tính phí
   - Production: Cần được Momo approve, có phí giao dịch

2. **Timeout:**
   - Payment request có thời hạn (thường 15 phút)
   - Nếu quá thời hạn, user cần tạo payment request mới

3. **Idempotency:**
   - Mỗi order chỉ nên tạo 1 payment request
   - Nếu đã có MomoOrderId, không tạo lại

4. **Error Handling:**
   - Luôn kiểm tra `resultCode` từ Momo
   - `resultCode = 0` = Success
   - Các mã khác = Failed

5. **Logging:**
   - Tất cả payment requests đều được log
   - Kiểm tra logs để debug nếu có vấn đề

---

## 🆘 Troubleshooting

### Lỗi: "Invalid signature"
- Kiểm tra Secret Key có đúng không
- Kiểm tra cách tính signature có đúng format không

### Lỗi: "Order not found"
- Kiểm tra MomoOrderId có đúng không
- Kiểm tra order có tồn tại trong database không

### Webhook không được gọi
- Kiểm tra URL webhook có đúng không
- Kiểm tra server có thể truy cập từ internet không (dùng ngrok nếu local)
- Kiểm tra firewall/security group

### QR Code không hiển thị
- Kiểm tra `qrCodeUrl` có hợp lệ không
- Thử mở `paymentUrl` trực tiếp trên browser

---

## 📞 Hỗ Trợ

- **Momo Developer Portal**: https://developers.momo.vn/
- **Momo API Documentation**: https://developers.momo.vn/docs/
- **Support**: support@momo.vn

---

**Chúc bạn tích hợp thành công! 🎉**

