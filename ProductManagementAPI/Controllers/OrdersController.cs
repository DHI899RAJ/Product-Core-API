using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;
using ProductManagementAPI.Models.DTOs;
using AutoMapper;

namespace ProductManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, IMapper mapper, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetAllOrders()
        {
            try
            {
                _logger.LogInformation("Fetching all orders");
                var orders = await _orderService.GetAllOrdersAsync();
                var responseDtos = _mapper.Map<IEnumerable<OrderResponseDto>>(orders);
                return Ok(responseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders");
                return BadRequest(new { message = "Error fetching orders", error = ex.Message });
            }
        }

        /// <summary>
        /// Get order by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderResponseDto>> GetOrderById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching order with ID: {OrderId}", id);
                var order = await _orderService.GetOrderByIdAsync(id);

                if (order == null)
                {
                    _logger.LogWarning("Order with ID: {OrderId} not found", id);
                    return NotFound(new { message = $"Order with ID {id} not found" });
                }

                var responseDto = _mapper.Map<OrderResponseDto>(order);
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when fetching order");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order");
                return BadRequest(new { message = "Error fetching order", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new order
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder(OrderCreateDto orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("Creating new order for customer: {CustomerEmail}", orderDto.CustomerEmail);
                var order = _mapper.Map<Order>(orderDto);
                var createdOrder = await _orderService.CreateOrderAsync(order);
                var responseDto = _mapper.Map<OrderResponseDto>(createdOrder);

                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, responseDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when creating order");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return BadRequest(new { message = "Error creating order", error = ex.Message });
            }
        }

        /// <summary>
        /// Update an order
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrder(int id, OrderUpdateDto orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("Updating order with ID: {OrderId}", id);
                var order = _mapper.Map<Order>(orderDto);
                var result = await _orderService.UpdateOrderAsync(id, order);

                if (!result)
                {
                    _logger.LogWarning("Order with ID: {OrderId} not found for update", id);
                    return NotFound(new { message = $"Order with ID {id} not found" });
                }

                return Ok(new { message = "Order updated successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when updating order");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Order not found for update");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order");
                return BadRequest(new { message = "Error updating order", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete an order
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                _logger.LogInformation("Deleting order with ID: {OrderId}", id);
                var result = await _orderService.DeleteOrderAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Order with ID: {OrderId} not found for deletion", id);
                    return NotFound(new { message = $"Order with ID {id} not found" });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when deleting order");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting order");
                return BadRequest(new { message = "Error deleting order", error = ex.Message });
            }
        }
    }
}
