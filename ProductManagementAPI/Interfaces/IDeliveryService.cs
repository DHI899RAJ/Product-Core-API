using ProductManagementAPI.Models;

namespace ProductManagementAPI.Interfaces
{
    public interface IDeliveryService
    {
        Task<IEnumerable<Delivery>> GetAllDeliveriesAsync();
        Task<Delivery?> GetDeliveryByIdAsync(int id);
        Task<IEnumerable<Delivery>> GetDeliveriesByOrderIdAsync(int orderId);
        Task<Delivery> CreateDeliveryAsync(Delivery delivery);
        Task<bool> UpdateDeliveryAsync(int id, Delivery delivery);
        Task<bool> DeleteDeliveryAsync(int id);
    }
}
