using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;

namespace ProductManagementAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _repository;

        public OrderService(IRepository<Order> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            ValidateOrder(order);

            order.OrderNumber = GenerateOrderNumber();
            order.OrderDate = DateTime.UtcNow;
            order.CreatedAt = DateTime.UtcNow;
            order.Status = "Pending";

            return await _repository.AddAsync(order);
        }

        public async Task<bool> UpdateOrderAsync(int id, Order order)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            var existingOrder = await _repository.GetByIdAsync(id);
            if (existingOrder == null)
                throw new InvalidOperationException($"Order with ID {id} not found");

            ValidateOrder(order);
            order.UpdatedAt = DateTime.UtcNow;
            order.Id = id;

            return await _repository.UpdateAsync(id, order);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            return await _repository.DeleteAsync(id);
        }

        private void ValidateOrder(Order order)
        {
            if (string.IsNullOrWhiteSpace(order.CustomerEmail))
                throw new ArgumentException("Customer email is required", nameof(order.CustomerEmail));

            if (string.IsNullOrWhiteSpace(order.CustomerName))
                throw new ArgumentException("Customer name is required", nameof(order.CustomerName));

            if (string.IsNullOrWhiteSpace(order.ShippingAddress))
                throw new ArgumentException("Shipping address is required", nameof(order.ShippingAddress));

            if (order.TotalAmount <= 0)
                throw new ArgumentException("Total amount must be greater than 0", nameof(order.TotalAmount));
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}
