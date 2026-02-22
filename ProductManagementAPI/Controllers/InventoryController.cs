using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;
using ProductManagementAPI.Models.DTOs;
using AutoMapper;

namespace ProductManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IInventoryService inventoryService, IMapper mapper, ILogger<InventoryController> logger)
        {
            _inventoryService = inventoryService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all inventory
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InventoryResponseDto>>> GetAllInventory()
        {
            try
            {
                _logger.LogInformation("Fetching all inventory");
                var inventory = await _inventoryService.GetAllInventoryAsync();
                var responseDtos = _mapper.Map<IEnumerable<InventoryResponseDto>>(inventory);
                return Ok(responseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching inventory");
                return BadRequest(new { message = "Error fetching inventory", error = ex.Message });
            }
        }

        /// <summary>
        /// Get inventory by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryResponseDto>> GetInventoryById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching inventory with ID: {InventoryId}", id);
                var inventory = await _inventoryService.GetInventoryByIdAsync(id);

                if (inventory == null)
                {
                    _logger.LogWarning("Inventory with ID: {InventoryId} not found", id);
                    return NotFound(new { message = $"Inventory with ID {id} not found" });
                }

                var responseDto = _mapper.Map<InventoryResponseDto>(inventory);
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when fetching inventory");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching inventory");
                return BadRequest(new { message = "Error fetching inventory", error = ex.Message });
            }
        }

        /// <summary>
        /// Get inventory by Product ID
        /// </summary>
        [HttpGet("product/{productId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryResponseDto>> GetInventoryByProductId(int productId)
        {
            try
            {
                _logger.LogInformation("Fetching inventory for product ID: {ProductId}", productId);
                var inventory = await _inventoryService.GetInventoryByProductIdAsync(productId);

                if (inventory == null)
                {
                    _logger.LogWarning("Inventory for product ID: {ProductId} not found", productId);
                    return NotFound(new { message = $"Inventory for product ID {productId} not found" });
                }

                var responseDto = _mapper.Map<InventoryResponseDto>(inventory);
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when fetching inventory");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching inventory");
                return BadRequest(new { message = "Error fetching inventory", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new inventory
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InventoryResponseDto>> CreateInventory(InventoryCreateDto inventoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("Creating new inventory for product ID: {ProductId}", inventoryDto.ProductId);
                var inventory = _mapper.Map<Inventory>(inventoryDto);
                var createdInventory = await _inventoryService.CreateInventoryAsync(inventory);
                var responseDto = _mapper.Map<InventoryResponseDto>(createdInventory);

                return CreatedAtAction(nameof(GetInventoryById), new { id = createdInventory.Id }, responseDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when creating inventory");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Product not found for inventory");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory");
                return BadRequest(new { message = "Error creating inventory", error = ex.Message });
            }
        }

        /// <summary>
        /// Update inventory
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateInventory(int id, InventoryUpdateDto inventoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("Updating inventory with ID: {InventoryId}", id);
                var inventory = _mapper.Map<Inventory>(inventoryDto);
                var result = await _inventoryService.UpdateInventoryAsync(id, inventory);

                if (!result)
                {
                    _logger.LogWarning("Inventory with ID: {InventoryId} not found for update", id);
                    return NotFound(new { message = $"Inventory with ID {id} not found" });
                }

                return Ok(new { message = "Inventory updated successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when updating inventory");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Inventory not found for update");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory");
                return BadRequest(new { message = "Error updating inventory", error = ex.Message });
            }
        }

        /// <summary>
        /// Update inventory quantity
        /// </summary>
        [HttpPatch("{id}/quantity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateInventoryQuantity(int id, [FromBody] int quantityChange)
        {
            try
            {
                _logger.LogInformation("Updating quantity for inventory ID: {InventoryId}, Change: {QuantityChange}", id, quantityChange);
                var result = await _inventoryService.UpdateQuantityAsync(id, quantityChange);

                if (!result)
                {
                    _logger.LogWarning("Inventory with ID: {InventoryId} not found for quantity update", id);
                    return NotFound(new { message = $"Inventory with ID {id} not found" });
                }

                return Ok(new { message = "Inventory quantity updated successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when updating quantity");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error updating inventory quantity");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory quantity");
                return BadRequest(new { message = "Error updating inventory quantity", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete inventory
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteInventory(int id)
        {
            try
            {
                _logger.LogInformation("Deleting inventory with ID: {InventoryId}", id);
                var result = await _inventoryService.DeleteInventoryAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Inventory with ID: {InventoryId} not found for deletion", id);
                    return NotFound(new { message = $"Inventory with ID {id} not found" });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when deleting inventory");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory");
                return BadRequest(new { message = "Error deleting inventory", error = ex.Message });
            }
        }
    }
}
