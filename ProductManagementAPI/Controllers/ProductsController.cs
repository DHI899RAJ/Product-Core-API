using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;
using ProductManagementAPI.Models.DTOs;

namespace ProductManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAllProducts()
        {
            _logger.LogInformation("Fetching all products");
            var products = await _productService.GetAllProductsAsync();
            var responseDtos = products.Select(MapToResponseDto);
            return Ok(responseDtos);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponseDto>> GetProductById(int id)
        {
            _logger.LogInformation("Fetching product with ID: {ProductId}", id);
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found", id);
                return NotFound();
            }

            return Ok(MapToResponseDto(product));
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct(ProductCreateDto productDto)
        {
            try
            {
                _logger.LogInformation("Creating new product: {ProductName}", productDto.Name);
                var product = MapToEntity(productDto);
                var createdProduct = await _productService.CreateProductAsync(product);
                var responseDto = MapToResponseDto(createdProduct);
                return CreatedAtAction(nameof(GetProductById), new { id = responseDto.Id }, responseDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("Validation error: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProduct(int id, ProductUpdateDto productDto)
        {
            try
            {
                _logger.LogInformation("Updating product with ID: {ProductId}", id);
                var product = MapToEntity(productDto);
                var updated = await _productService.UpdateProductAsync(id, product);

                if (!updated)
                {
                    _logger.LogWarning("Product with ID: {ProductId} not found for update", id);
                    return NotFound();
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError("Validation error: {Message}", ex.Message);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Product not found: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation("Deleting product with ID: {ProductId}", id);
            var deleted = await _productService.DeleteProductAsync(id);

            if (!deleted)
            {
                _logger.LogWarning("Product with ID: {ProductId} not found for deletion", id);
                return NotFound();
            }

            return NoContent();
        }

        // Manual mapping methods
        private Product MapToEntity(ProductCreateDto dto)
        {
            return new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId
            };
        }

        private Product MapToEntity(ProductUpdateDto dto)
        {
            return new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId
            };
        }

        private ProductResponseDto MapToResponseDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
