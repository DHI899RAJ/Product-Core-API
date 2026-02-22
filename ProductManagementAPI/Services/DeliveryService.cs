using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;

namespace ProductManagementAPI.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IRepository<Delivery> _repository;
        private readonly IRepository<Order> _orderRepository;

        public DeliveryService(IRepository<Delivery> repository, IRepository<Order> orderRepository)
        {
            _repository = repository;
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<Delivery>> GetAllDeliveriesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Delivery?> GetDeliveryByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Delivery>> GetDeliveriesByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Order ID must be greater than 0", nameof(orderId));

            var allDeliveries = await _repository.GetAllAsync();
            return allDeliveries.Where(d => d.OrderId == orderId);
        }

        public async Task<Delivery> CreateDeliveryAsync(Delivery delivery)
        {
            ValidateDelivery(delivery);

            var order = await _orderRepository.GetByIdAsync(delivery.OrderId);
            if (order == null)
                throw new InvalidOperationException($"Order with ID {delivery.OrderId} not found");

            delivery.TrackingNumber = GenerateTrackingNumber();
            delivery.CreatedAt = DateTime.UtcNow;
            delivery.Status = "Pending";

            return await _repository.AddAsync(delivery);
        }

        public async Task<bool> UpdateDeliveryAsync(int id, Delivery delivery)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            var existingDelivery = await _repository.GetByIdAsync(id);
            if (existingDelivery == null)
                throw new InvalidOperationException($"Delivery with ID {id} not found");

            ValidateDelivery(delivery);
            delivery.UpdatedAt = DateTime.UtcNow;
            delivery.Id = id;

            return await _repository.UpdateAsync(id, delivery);
        }

        public async Task<bool> DeleteDeliveryAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            return await _repository.DeleteAsync(id);
        }

        private void ValidateDelivery(Delivery delivery)
        {
            if (delivery.OrderId <= 0)
                throw new ArgumentException("Order ID is required", nameof(delivery.OrderId));

            if (string.IsNullOrWhiteSpace(delivery.CarrierName))
                throw new ArgumentException("Carrier name is required", nameof(delivery.CarrierName));

            if (string.IsNullOrWhiteSpace(delivery.DeliveryAddress))
                throw new ArgumentException("Delivery address is required", nameof(delivery.DeliveryAddress));
        }

        private string GenerateTrackingNumber()
        {
            return $"TRK-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        }
    }
}
