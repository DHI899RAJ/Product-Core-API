using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;

namespace ProductManagementAPI.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IRepository<Inventory> _repository;
        private readonly IRepository<Product> _productRepository;

        public InventoryService(IRepository<Inventory> repository, IRepository<Product> productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Inventory>> GetAllInventoryAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Inventory?> GetInventoryByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public async Task<Inventory?> GetInventoryByProductIdAsync(int productId)
        {
            if (productId <= 0)
                throw new ArgumentException("Product ID must be greater than 0", nameof(productId));

            var allInventory = await _repository.GetAllAsync();
            return allInventory.FirstOrDefault(i => i.ProductId == productId);
        }

        public async Task<Inventory> CreateInventoryAsync(Inventory inventory)
        {
            ValidateInventory(inventory);

            var product = await _productRepository.GetByIdAsync(inventory.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product with ID {inventory.ProductId} not found");

            inventory.QuantityAvailable = inventory.QuantityOnHand - inventory.QuantityReserved;
            inventory.CreatedAt = DateTime.UtcNow;

            return await _repository.AddAsync(inventory);
        }

        public async Task<bool> UpdateInventoryAsync(int id, Inventory inventory)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            var existingInventory = await _repository.GetByIdAsync(id);
            if (existingInventory == null)
                throw new InvalidOperationException($"Inventory with ID {id} not found");

            ValidateInventory(inventory);
            inventory.QuantityAvailable = inventory.QuantityOnHand - inventory.QuantityReserved;
            inventory.UpdatedAt = DateTime.UtcNow;
            inventory.Id = id;

            return await _repository.UpdateAsync(id, inventory);
        }

        public async Task<bool> DeleteInventoryAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> UpdateQuantityAsync(int id, int quantityChange)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            var inventory = await _repository.GetByIdAsync(id);
            if (inventory == null)
                throw new InvalidOperationException($"Inventory with ID {id} not found");

            inventory.QuantityOnHand += quantityChange;
            inventory.QuantityAvailable = inventory.QuantityOnHand - inventory.QuantityReserved;

            if (inventory.QuantityOnHand < 0)
                throw new InvalidOperationException("Quantity cannot be negative");

            inventory.UpdatedAt = DateTime.UtcNow;

            return await _repository.UpdateAsync(id, inventory);
        }

        private void ValidateInventory(Inventory inventory)
        {
            if (inventory.ProductId <= 0)
                throw new ArgumentException("Product ID is required", nameof(inventory.ProductId));

            if (inventory.QuantityOnHand < 0)
                throw new ArgumentException("Quantity on hand cannot be negative", nameof(inventory.QuantityOnHand));

            if (inventory.QuantityReserved < 0)
                throw new ArgumentException("Quantity reserved cannot be negative", nameof(inventory.QuantityReserved));

            if (inventory.ReorderLevel < 0)
                throw new ArgumentException("Reorder level cannot be negative", nameof(inventory.ReorderLevel));

            if (string.IsNullOrWhiteSpace(inventory.WarehouseLocation))
                throw new ArgumentException("Warehouse location is required", nameof(inventory.WarehouseLocation));
        }
    }
}
