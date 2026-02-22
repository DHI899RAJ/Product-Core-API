using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;

namespace ProductManagementAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;

        public ProductService(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            ValidateProduct(product);

            product.CreatedAt = DateTime.UtcNow;
            return await _repository.AddAsync(product);
        }

        public async Task<bool> UpdateProductAsync(int id, Product product)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            // Verify product exists
            var existingProduct = await _repository.GetByIdAsync(id);
            if (existingProduct == null)
                throw new InvalidOperationException($"Product with ID {id} not found");

            ValidateProduct(product);
            product.UpdatedAt = DateTime.UtcNow;

            return await _repository.UpdateAsync(id, product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            return await _repository.DeleteAsync(id);
        }

        private void ValidateProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("Product name is required", nameof(product.Name));

            if (product.Name.Length > 200)
                throw new ArgumentException("Product name cannot exceed 200 characters", nameof(product.Name));

            if (product.Price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(product.Price));

            if (product.Quantity < 0)
                throw new ArgumentException("Quantity cannot be negative", nameof(product.Quantity));

            if (product.CategoryId <= 0)
                throw new ArgumentException("Valid category is required", nameof(product.CategoryId));

            if (product.SupplierId <= 0)
                throw new ArgumentException("Valid supplier is required", nameof(product.SupplierId));
        }
    }
}
