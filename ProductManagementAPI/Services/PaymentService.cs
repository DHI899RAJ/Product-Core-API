using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;

namespace ProductManagementAPI.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository<Payment> _repository;
        private readonly IRepository<Order> _orderRepository;

        public PaymentService(IRepository<Payment> repository, IRepository<Order> orderRepository)
        {
            _repository = repository;
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Payment?> GetPaymentByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Order ID must be greater than 0", nameof(orderId));

            var allPayments = await _repository.GetAllAsync();
            return allPayments.Where(p => p.OrderId == orderId);
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            ValidatePayment(payment);

            var order = await _orderRepository.GetByIdAsync(payment.OrderId);
            if (order == null)
                throw new InvalidOperationException($"Order with ID {payment.OrderId} not found");

            payment.PaymentDate = DateTime.UtcNow;
            payment.CreatedAt = DateTime.UtcNow;
            payment.Status = "Pending";

            return await _repository.AddAsync(payment);
        }

        public async Task<bool> UpdatePaymentAsync(int id, Payment payment)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            var existingPayment = await _repository.GetByIdAsync(id);
            if (existingPayment == null)
                throw new InvalidOperationException($"Payment with ID {id} not found");

            ValidatePayment(payment);
            payment.UpdatedAt = DateTime.UtcNow;
            payment.Id = id;

            return await _repository.UpdateAsync(id, payment);
        }

        public async Task<bool> DeletePaymentAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            return await _repository.DeleteAsync(id);
        }

        private void ValidatePayment(Payment payment)
        {
            if (payment.OrderId <= 0)
                throw new ArgumentException("Order ID is required", nameof(payment.OrderId));

            if (payment.Amount <= 0)
                throw new ArgumentException("Payment amount must be greater than 0", nameof(payment.Amount));

            if (string.IsNullOrWhiteSpace(payment.PaymentMethod))
                throw new ArgumentException("Payment method is required", nameof(payment.PaymentMethod));

            if (string.IsNullOrWhiteSpace(payment.TransactionId))
                throw new ArgumentException("Transaction ID is required", nameof(payment.TransactionId));
        }
    }
}
