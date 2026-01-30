namespace quanlybanthuoc.Dtos.Payment
{
    public class PaymentResponse
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string QrCodeUrl { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public string? DeepLink { get; set; }
    }

    public class PaymentStatusResponse
    {
        public int OrderId { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
        public string? MomoOrderId { get; set; }
        public string? MomoTransactionId { get; set; }
        public DateTime? NgayThanhToan { get; set; }
        public string? Message { get; set; }
    }
}

