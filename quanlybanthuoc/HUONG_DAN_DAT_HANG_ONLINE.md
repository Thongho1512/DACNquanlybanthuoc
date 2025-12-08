# Hướng Dẫn Chức Năng Đặt Hàng Online Cho Khách Hàng

## 📋 Tổng Quan

Hệ thống hỗ trợ **2 cách đặt hàng online**:

1. **Guest Checkout** - Đặt hàng không cần đăng nhập
   - Khách hàng chỉ cần cung cấp: Tên, Số điện thoại
   - **KHÔNG tích điểm thưởng**
   - Tự động tạo hoặc tìm khách hàng theo SDT

2. **Đặt hàng với tài khoản** - Khách hàng đã đăng nhập
   - Khách hàng đăng nhập bằng SDT + OTP
   - **CÓ tích điểm thưởng**
   - Có thể sử dụng điểm tích lũy để giảm giá

---

## 🔐 Đăng Ký & Đăng Nhập Khách Hàng

### 1. Đăng Ký Tài Khoản

**Endpoint:** `POST /api/v1/auth/customer/register`

**Request:**
```json
{
  "tenKhachHang": "Nguyễn Văn A",
  "sdt": "0901234567"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Thành công.",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "khachHangDto": {
      "id": 1,
      "tenKhachHang": "Nguyễn Văn A",
      "sdt": "0901234567",
      "diemTichLuy": 0
    }
  }
}
```

### 2. Gửi OTP

**Endpoint:** `POST /api/v1/auth/customer/send-otp`

**Request:**
```json
{
  "sdt": "0901234567"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Thành công.",
  "data": "OTP đã được gửi đến số điện thoại của bạn."
}
```

**Lưu ý:** 
- Trong môi trường development, OTP được log ra console
- Trong production, cần tích hợp SMS service (Twilio, AWS SNS, etc.)

### 3. Đăng Nhập

**Endpoint:** `POST /api/v1/auth/customer/login`

**Request:**
```json
{
  "sdt": "0901234567",
  "otp": "123456"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Thành công.",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "khachHangDto": {
      "id": 1,
      "tenKhachHang": "Nguyễn Văn A",
      "sdt": "0901234567",
      "diemTichLuy": 150
    }
  }
}
```

**Lưu ý:**
- Token có thời hạn 24 giờ
- Token chứa claim `CustomerId` để xác định khách hàng
- Role: `CUSTOMER`

---

## 🛒 Đặt Hàng Online

### 1. Guest Checkout (Không cần đăng nhập)

**Endpoint:** `POST /api/v1/customer/orders`

**Headers:**
```
Content-Type: application/json
```

**Request:**
```json
{
  "tenKhachHang": "Nguyễn Văn A",
  "sdt": "0901234567",
  "idchiNhanh": 1,
  "idphuongThucTt": 1,
  "loaiDonHang": "GIAO_HANG",
  "diaChiGiaoHang": "123 Đường ABC, Quận 1, TP.HCM",
  "soDienThoaiNguoiNhan": "0901234567",
  "tenNguoiNhan": "Nguyễn Văn A",
  "chiTietDonHangs": [
    {
      "idthuoc": 1,
      "soLuong": 2,
      "donGia": 50000
    },
    {
      "idthuoc": 2,
      "soLuong": 1,
      "donGia": 75000
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "message": "Thành công.",
  "data": {
    "id": 123,
    "idkhachHang": 1,
    "idchiNhanh": 1,
    "tongTien": 175000,
    "tienGiamGia": 0,
    "thanhTien": 175000,
    "trangThaiThanhToan": "PENDING_PAYMENT",
    "loaiDonHang": "GIAO_HANG"
  }
}
```

**Lưu ý:**
- Không cần `Authorization` header
- Hệ thống tự động tìm hoặc tạo `KhachHang` theo SDT
- **KHÔNG tích điểm** vì là guest checkout

### 2. Đặt Hàng Với Tài Khoản (Đã đăng nhập)

**Endpoint:** `POST /api/v1/customer/orders`

**Headers:**
```
Authorization: Bearer {customer_token}
Content-Type: application/json
```

**Request:**
```json
{
  "idchiNhanh": 1,
  "idphuongThucTt": 2,
  "loaiDonHang": "GIAO_HANG",
  "diaChiGiaoHang": "123 Đường ABC, Quận 1, TP.HCM",
  "soDienThoaiNguoiNhan": "0901234567",
  "tenNguoiNhan": "Nguyễn Văn A",
  "chiTietDonHangs": [
    {
      "idthuoc": 1,
      "soLuong": 2,
      "donGia": 50000
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "message": "Thành công.",
  "data": {
    "id": 124,
    "idkhachHang": 1,
    "tongTien": 100000,
    "tienGiamGia": 5000,
    "thanhTien": 95000,
    "trangThaiThanhToan": "PENDING_PAYMENT",
    "loaiDonHang": "GIAO_HANG"
  }
}
```

**Lưu ý:**
- Cần `Authorization` header với token từ login
- Hệ thống tự động lấy `idKhachHang` từ token
- **CÓ tích điểm** nếu thanh toán thành công
- Có thể sử dụng điểm tích lũy để giảm giá (tự động)

---

## 💳 Thanh Toán

### 1. Tạo Payment Request (Momo QR Code)

**Endpoint:** `POST /api/v1/payments/create`

**Headers:**
```
Content-Type: application/json
Authorization: Bearer {token} (optional - cho guest checkout)
```

**Request:**
```json
{
  "orderId": 123,
  "amount": 175000,
  "paymentMethodCode": "MOMO"
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
    "orderId": "ORDER_123_1234567890"
  }
}
```

**Lưu ý:**
- Endpoint này **AllowAnonymous** - cho phép guest checkout
- Frontend hiển thị QR Code từ `qrCodeUrl`

### 2. Kiểm Tra Trạng Thái Thanh Toán

**Endpoint:** `GET /api/v1/payments/{orderId}/status`

**Headers:**
```
Authorization: Bearer {token} (optional)
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
    "ngayThanhToan": "2024-01-15T10:30:00"
  }
}
```

---

## 📊 Luồng Đặt Hàng Online

### Luồng Guest Checkout:

```
1. Khách hàng xem sản phẩm (không cần đăng nhập)
   GET /api/v1/customer/medicines/search

2. Khách hàng đặt hàng (không cần đăng nhập)
   POST /api/v1/customer/orders
   → Hệ thống tự động tìm/tạo KhachHang theo SDT
   → KHÔNG tích điểm

3. Tạo payment request
   POST /api/v1/payments/create
   → Nhận QR Code

4. Thanh toán Momo
   → Quét QR Code và thanh toán

5. Momo gọi webhook
   POST /api/v1/payments/notify
   → Cập nhật trạng thái PAID
   → Tạo DonGiaoHang nếu LoaiDonHang = "GIAO_HANG"

6. Kiểm tra trạng thái
   GET /api/v1/payments/{orderId}/status
```

### Luồng Đặt Hàng Với Tài Khoản:

```
1. Khách hàng đăng nhập
   POST /api/v1/auth/customer/login
   → Nhận token với CustomerId

2. Khách hàng đặt hàng (có token)
   POST /api/v1/customer/orders
   Authorization: Bearer {token}
   → Hệ thống lấy CustomerId từ token
   → CÓ tích điểm sau khi thanh toán thành công

3. Tạo payment request
   POST /api/v1/payments/create
   → Nhận QR Code

4. Thanh toán Momo
   → Quét QR Code và thanh toán

5. Momo gọi webhook
   → Cập nhật trạng thái PAID
   → TÍCH ĐIỂM cho khách hàng
   → Tạo DonGiaoHang nếu cần

6. Kiểm tra trạng thái
   GET /api/v1/payments/{orderId}/status
```

---

## 💎 Hệ Thống Tích Điểm

### Quy Tắc Tích Điểm:

- **Chỉ tích điểm khi khách hàng đã đăng nhập** và thanh toán thành công
- **Guest checkout KHÔNG tích điểm**
- Tỷ lệ tích điểm: **10,000 VNĐ = 1 điểm**
- Tỷ lệ quy đổi: **1 điểm = 1,000 VNĐ** (giảm giá)
- Tối thiểu sử dụng: **10 điểm**
- Giảm giá tối đa: **50% giá trị đơn hàng**

### Ví Dụ:

**Khách hàng đã đăng nhập:**
- Đơn hàng: 100,000 VNĐ
- Thanh toán thành công → Tích điểm: **10 điểm** (100,000 / 10,000)
- Đơn hàng tiếp theo: 50,000 VNĐ
- Có 10 điểm → Giảm giá: **10,000 VNĐ** (10 điểm × 1,000)
- Thành tiền: **40,000 VNĐ**
- Tích điểm mới: **4 điểm** (40,000 / 10,000)

**Guest checkout:**
- Đơn hàng: 100,000 VNĐ
- Thanh toán thành công → **KHÔNG tích điểm**

---

## 🔍 API Endpoints Tổng Hợp

### Authentication (Khách hàng)
- `POST /api/v1/auth/customer/register` - Đăng ký
- `POST /api/v1/auth/customer/login` - Đăng nhập
- `POST /api/v1/auth/customer/send-otp` - Gửi OTP
- `POST /api/v1/auth/customer/verify-otp` - Xác thực OTP

### Đặt Hàng
- `POST /api/v1/customer/orders` - Đặt hàng (AllowAnonymous)
- `GET /api/v1/customer/orders` - Lịch sử đơn hàng (Require Auth)
- `GET /api/v1/customer/orders/{orderId}` - Chi tiết đơn hàng (Require Auth)

### Thanh Toán
- `POST /api/v1/payments/create` - Tạo payment request (AllowAnonymous)
- `GET /api/v1/payments/{orderId}/status` - Trạng thái thanh toán (AllowAnonymous)
- `POST /api/v1/payments/notify` - Webhook Momo (AllowAnonymous)

### Xem Sản Phẩm (Không cần đăng nhập)
- `GET /api/v1/customer/medicines/featured` - Sản phẩm nổi bật
- `GET /api/v1/customer/medicines/search` - Tìm kiếm
- `GET /api/v1/customer/medicines/{id}` - Chi tiết sản phẩm
- `GET /api/v1/customer/categories` - Danh mục
- `GET /api/v1/customer/branches` - Chi nhánh

---

## 📝 Lưu Ý Quan Trọng

1. **Guest Checkout:**
   - Khách hàng có thể đặt hàng mà không cần đăng nhập
   - Chỉ cần cung cấp: Tên, Số điện thoại
   - Hệ thống tự động tìm hoặc tạo KhachHang
   - **KHÔNG tích điểm**

2. **Đặt Hàng Với Tài Khoản:**
   - Khách hàng cần đăng nhập trước
   - Sử dụng token trong header `Authorization: Bearer {token}`
   - **CÓ tích điểm** sau khi thanh toán thành công
   - Có thể sử dụng điểm tích lũy để giảm giá

3. **OTP:**
   - Hiện tại OTP được lưu trong memory (Dictionary)
   - Trong production, nên dùng Redis hoặc database
   - Cần tích hợp SMS service để gửi OTP thật

4. **Token:**
   - Token cho khách hàng có claim `CustomerId`
   - Token cho nhân viên có claim `NameIdentifier` (UserId)
   - Phân biệt bằng Role: `CUSTOMER` vs `ADMIN/MANAGER/STAFF`

---

## 🧪 Testing

### Test Guest Checkout:

```bash
# 1. Xem sản phẩm (không cần đăng nhập)
GET /api/v1/customer/medicines/search?searchTerm=paracetamol

# 2. Đặt hàng (không cần đăng nhập)
POST /api/v1/customer/orders
{
  "tenKhachHang": "Nguyễn Văn A",
  "sdt": "0901234567",
  "idchiNhanh": 1,
  "idphuongThucTt": 2,
  "loaiDonHang": "GIAO_HANG",
  "chiTietDonHangs": [...]
}

# 3. Tạo payment
POST /api/v1/payments/create
{
  "orderId": 123,
  "amount": 100000,
  "paymentMethodCode": "MOMO"
}

# 4. Kiểm tra trạng thái
GET /api/v1/payments/123/status
```

### Test Đặt Hàng Với Tài Khoản:

```bash
# 1. Đăng ký
POST /api/v1/auth/customer/register
{
  "tenKhachHang": "Nguyễn Văn A",
  "sdt": "0901234567"
}

# 2. Đặt hàng (có token)
POST /api/v1/customer/orders
Authorization: Bearer {token}
{
  "idchiNhanh": 1,
  "idphuongThucTt": 2,
  "chiTietDonHangs": [...]
}

# 3. Thanh toán và kiểm tra tích điểm
```

---

**Chức năng đặt hàng online đã hoàn thiện! 🎉**

