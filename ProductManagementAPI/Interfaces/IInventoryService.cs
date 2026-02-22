using ProductManagementAPI.Models;

namespace ProductManagementAPI.Interfaces
{
    public interface IInventoryService
    {
        Task<IEnumerable<Inventory>> GetAllInventoryAsync();
        Task<Inventory?> GetInventoryByIdAsync(int id);
        Task<Inventory?> GetInventoryByProductIdAsync(int productId);
        Task<Inventory> CreateInventoryAsync(Inventory inventory);
        Task<bool> UpdateInventoryAsync(int id, Inventory inventory);
        Task<bool> DeleteInventoryAsync(int id);
        Task<bool> UpdateQuantityAsync(int id, int quantityChange);
    }
}
