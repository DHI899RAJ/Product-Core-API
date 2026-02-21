using ProductManagementAPI.Models;

namespace ProductManagementAPI.Data
{
    /// <summary>
    /// In-memory repository for demonstration purposes.
    /// Replace with actual database context (Entity Framework Core) in production.
    /// </summary>
    public class ProductRepository
    {
        private static List<Product> _products = new()
        {
            new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, Quantity = 5 },
            new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 29.99m, Quantity = 50 },
            new Product { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard", Price = 129.99m, Quantity = 20 }
        };

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await Task.FromResult(_products);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
        }

        public async Task<Product> AddAsync(Product product)
        {
            product.Id = _products.Any() ? _products.Max(p => p.Id) + 1 : 1;
            product.CreatedAt = DateTime.UtcNow;
            _products.Add(product);
            return await Task.FromResult(product);
        }

        public async Task<bool> UpdateAsync(int id, Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null) return await Task.FromResult(false);

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Quantity = product.Quantity;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product == null) return await Task.FromResult(false);

            _products.Remove(product);
            return await Task.FromResult(true);
        }
    }
}
