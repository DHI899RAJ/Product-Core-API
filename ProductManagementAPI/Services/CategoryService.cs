using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;

namespace ProductManagementAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _repository;

        public CategoryService(IRepository<Category> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            return await _repository.GetByIdAsync(id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            ValidateCategory(category);

            category.CreatedAt = DateTime.UtcNow;
            return await _repository.AddAsync(category);
        }

        public async Task<bool> UpdateCategoryAsync(int id, Category category)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            var existingCategory = await _repository.GetByIdAsync(id);
            if (existingCategory == null)
                throw new InvalidOperationException($"Category with ID {id} not found");

            ValidateCategory(category);
            category.UpdatedAt = DateTime.UtcNow;
            category.Id = id;

            return await _repository.UpdateAsync(id, category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Id must be greater than 0", nameof(id));

            return await _repository.DeleteAsync(id);
        }

        private void ValidateCategory(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name is required", nameof(category.Name));
        }
    }
}
