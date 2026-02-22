using ProductManagementAPI.Models;

namespace ProductManagementAPI.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
        Task<Payment?> GetPaymentByIdAsync(int id);
        Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId);
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<bool> UpdatePaymentAsync(int id, Payment payment);
        Task<bool> DeletePaymentAsync(int id);
    }
}
