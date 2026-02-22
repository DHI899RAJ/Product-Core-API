using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;
using ProductManagementAPI.Models.DTOs;
using AutoMapper;

namespace ProductManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, IMapper mapper, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAllCategories()
        {
            try
            {
                _logger.LogInformation("Fetching all categories");
                var categories = await _categoryService.GetAllCategoriesAsync();
                var responseDtos = _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
                return Ok(responseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories");
                return BadRequest(new { message = "Error fetching categories", error = ex.Message });
            }
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryResponseDto>> GetCategoryById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching category with ID: {CategoryId}", id);
                var category = await _categoryService.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    _logger.LogWarning("Category with ID: {CategoryId} not found", id);
                    return NotFound(new { message = $"Category with ID {id} not found" });
                }

                var responseDto = _mapper.Map<CategoryResponseDto>(category);
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when fetching category");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category");
                return BadRequest(new { message = "Error fetching category", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CategoryResponseDto>> CreateCategory(CategoryCreateDto categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("Creating new category: {CategoryName}", categoryDto.Name);
                var category = _mapper.Map<Category>(categoryDto);
                var createdCategory = await _categoryService.CreateCategoryAsync(category);
                var responseDto = _mapper.Map<CategoryResponseDto>(createdCategory);

                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, responseDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when creating category");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return BadRequest(new { message = "Error creating category", error = ex.Message });
            }
        }

        /// <summary>
        /// Update a category
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCategory(int id, CategoryUpdateDto categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("Updating category with ID: {CategoryId}", id);
                var category = _mapper.Map<Category>(categoryDto);
                var result = await _categoryService.UpdateCategoryAsync(id, category);

                if (!result)
                {
                    _logger.LogWarning("Category with ID: {CategoryId} not found for update", id);
                    return NotFound(new { message = $"Category with ID {id} not found" });
                }

                return Ok(new { message = "Category updated successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when updating category");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Category not found for update");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                return BadRequest(new { message = "Error updating category", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
                var result = await _categoryService.DeleteCategoryAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Category with ID: {CategoryId} not found for deletion", id);
                    return NotFound(new { message = $"Category with ID {id} not found" });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when deleting category");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                return BadRequest(new { message = "Error deleting category", error = ex.Message });
            }
        }
    }
}
