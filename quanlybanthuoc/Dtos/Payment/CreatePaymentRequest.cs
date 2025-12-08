namespace quanlybanthuoc.Dtos.Payment
{
    public class CreatePaymentRequest
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethodCode { get; set; }
        public string? ReturnUrl { get; set; }
        public string? NotifyUrl { get; set; }
    }
}

