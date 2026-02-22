using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;
using ProductManagementAPI.Models.DTOs;
using AutoMapper;

namespace ProductManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveriesController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;
        private readonly IMapper _mapper;
        private readonly ILogger<DeliveriesController> _logger;

        public DeliveriesController(IDeliveryService deliveryService, IMapper mapper, ILogger<DeliveriesController> logger)
        {
            _deliveryService = deliveryService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all deliveries
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DeliveryResponseDto>>> GetAllDeliveries()
        {
            try
            {
                _logger.LogInformation("Fetching all deliveries");
                var deliveries = await _deliveryService.GetAllDeliveriesAsync();
                var responseDtos = _mapper.Map<IEnumerable<DeliveryResponseDto>>(deliveries);
                return Ok(responseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching deliveries");
                return BadRequest(new { message = "Error fetching deliveries", error = ex.Message });
            }
        }

        /// <summary>
        /// Get delivery by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DeliveryResponseDto>> GetDeliveryById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching delivery with ID: {DeliveryId}", id);
                var delivery = await _deliveryService.GetDeliveryByIdAsync(id);

                if (delivery == null)
                {
                    _logger.LogWarning("Delivery with ID: {DeliveryId} not found", id);
                    return NotFound(new { message = $"Delivery with ID {id} not found" });
                }

                var responseDto = _mapper.Map<DeliveryResponseDto>(delivery);
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when fetching delivery");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching delivery");
                return BadRequest(new { message = "Error fetching delivery", error = ex.Message });
            }
        }

        /// <summary>
        /// Get deliveries by Order ID
        /// </summary>
        [HttpGet("order/{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DeliveryResponseDto>>> GetDeliveriesByOrderId(int orderId)
        {
            try
            {
                _logger.LogInformation("Fetching deliveries for order ID: {OrderId}", orderId);
                var deliveries = await _deliveryService.GetDeliveriesByOrderIdAsync(orderId);
                var responseDtos = _mapper.Map<IEnumerable<DeliveryResponseDto>>(deliveries);
                return Ok(responseDtos);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when fetching deliveries");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching deliveries");
                return BadRequest(new { message = "Error fetching deliveries", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new delivery
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DeliveryResponseDto>> CreateDelivery(DeliveryCreateDto deliveryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("Creating new delivery for order ID: {OrderId}", deliveryDto.OrderId);
                var delivery = _mapper.Map<Delivery>(deliveryDto);
                var createdDelivery = await _deliveryService.CreateDeliveryAsync(delivery);
                var responseDto = _mapper.Map<DeliveryResponseDto>(createdDelivery);

                return CreatedAtAction(nameof(GetDeliveryById), new { id = createdDelivery.Id }, responseDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when creating delivery");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Order not found for delivery");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating delivery");
                return BadRequest(new { message = "Error creating delivery", error = ex.Message });
            }
        }

        /// <summary>
        /// Update a delivery
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDelivery(int id, DeliveryUpdateDto deliveryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("Updating delivery with ID: {DeliveryId}", id);
                var delivery = _mapper.Map<Delivery>(deliveryDto);
                var result = await _deliveryService.UpdateDeliveryAsync(id, delivery);

                if (!result)
                {
                    _logger.LogWarning("Delivery with ID: {DeliveryId} not found for update", id);
                    return NotFound(new { message = $"Delivery with ID {id} not found" });
                }

                return Ok(new { message = "Delivery updated successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when updating delivery");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Delivery not found for update");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating delivery");
                return BadRequest(new { message = "Error updating delivery", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a delivery
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDelivery(int id)
        {
            try
            {
                _logger.LogInformation("Deleting delivery with ID: {DeliveryId}", id);
                var result = await _deliveryService.DeleteDeliveryAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Delivery with ID: {DeliveryId} not found for deletion", id);
                    return NotFound(new { message = $"Delivery with ID {id} not found" });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when deleting delivery");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting delivery");
                return BadRequest(new { message = "Error deleting delivery", error = ex.Message });
            }
        }
    }
}
