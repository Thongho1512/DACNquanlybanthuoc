using quanlybanthuoc.Dtos.Payment;

namespace quanlybanthuoc.Services
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
        Task<bool> ProcessPaymentCallbackAsync(string orderId, Dictionary<string, string> callbackData);
        Task<PaymentStatusResponse> GetPaymentStatusAsync(int orderId);
    }
}

