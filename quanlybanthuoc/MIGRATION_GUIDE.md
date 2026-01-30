# Hướng Dẫn Tạo Migration

## Tạo Migration cho Payment Fields

Sau khi đã thêm các field mới vào `DonHang` entity, bạn cần tạo và áp dụng migration:

### Bước 1: Tạo Migration

Mở Terminal/PowerShell tại thư mục project và chạy:

```bash
dotnet ef migrations add AddPaymentFieldsToDonHang
```

Lệnh này sẽ tạo file migration mới trong thư mục `Migrations/`.

### Bước 2: Kiểm tra Migration File

Mở file migration vừa tạo (ví dụ: `Migrations/20240115000000_AddPaymentFieldsToDonHang.cs`) và kiểm tra:

```csharp
migrationBuilder.AddColumn<string>(
    name: "TrangThaiThanhToan",
    table: "DonHang",
    type: "nvarchar(50)",
    maxLength: 50,
    nullable: true,
    defaultValue: "PENDING_PAYMENT");

migrationBuilder.AddColumn<string>(
    name: "MomoOrderId",
    table: "DonHang",
    type: "nvarchar(100)",
    maxLength: 100,
    nullable: true);

migrationBuilder.AddColumn<string>(
    name: "MomoTransactionId",
    table: "DonHang",
    type: "nvarchar(100)",
    maxLength: 100,
    nullable: true);

migrationBuilder.AddColumn<DateTime>(
    name: "NgayThanhToan",
    table: "DonHang",
    type: "datetime2",
    nullable: true);
```

### Bước 3: Áp dụng Migration

```bash
dotnet ef database update
```

Lệnh này sẽ cập nhật database với các field mới.

### Bước 4: Kiểm tra Database

Sau khi migration thành công, kiểm tra bảng `DonHang` trong SQL Server:

```sql
SELECT 
    ID,
    TrangThaiThanhToan,
    MomoOrderId,
    MomoTransactionId,
    NgayThanhToan
FROM DonHang
```

### Nếu có lỗi Migration

Nếu gặp lỗi khi chạy migration:

1. **Kiểm tra connection string** trong `appsettings.json`
2. **Kiểm tra database có tồn tại** không
3. **Xóa migration** nếu cần:
   ```bash
   dotnet ef migrations remove
   ```
4. **Tạo lại migration**:
   ```bash
   dotnet ef migrations add AddPaymentFieldsToDonHang
   dotnet ef database update
   ```

### Rollback Migration (nếu cần)

Nếu muốn rollback migration:

```bash
dotnet ef database update PreviousMigrationName
```

Hoặc xóa migration:

```bash
dotnet ef migrations remove
```

---

**Lưu ý:** Luôn backup database trước khi chạy migration trong production!

