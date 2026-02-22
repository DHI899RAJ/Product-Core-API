using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;
using ProductManagementAPI.Models.DTOs;
using AutoMapper;

namespace ProductManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IPaymentService paymentService, IMapper mapper, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Get all payments
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentResponseDto>>> GetAllPayments()
        {
            try
            {
                _logger.LogInformation("Fetching all payments");
                var payments = await _paymentService.GetAllPaymentsAsync();
                var responseDtos = _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
                return Ok(responseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payments");
                return BadRequest(new { message = "Error fetching payments", error = ex.Message });
            }
        }

        /// <summary>
        /// Get payment by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentResponseDto>> GetPaymentById(int id)
        {
            try
            {
                _logger.LogInformation("Fetching payment with ID: {PaymentId}", id);
                var payment = await _paymentService.GetPaymentByIdAsync(id);

                if (payment == null)
                {
                    _logger.LogWarning("Payment with ID: {PaymentId} not found", id);
                    return NotFound(new { message = $"Payment with ID {id} not found" });
                }

                var responseDto = _mapper.Map<PaymentResponseDto>(payment);
                return Ok(responseDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when fetching payment");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payment");
                return BadRequest(new { message = "Error fetching payment", error = ex.Message });
            }
        }

        /// <summary>
        /// Get payments by Order ID
        /// </summary>
        [HttpGet("order/{orderId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PaymentResponseDto>>> GetPaymentsByOrderId(int orderId)
        {
            try
            {
                _logger.LogInformation("Fetching payments for order ID: {OrderId}", orderId);
                var payments = await _paymentService.GetPaymentsByOrderIdAsync(orderId);
                var responseDtos = _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
                return Ok(responseDtos);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when fetching payments");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching payments");
                return BadRequest(new { message = "Error fetching payments", error = ex.Message });
            }
        }

        /// <summary>
        /// Create a new payment
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentResponseDto>> CreatePayment(PaymentCreateDto paymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("Creating new payment for order ID: {OrderId}", paymentDto.OrderId);
                var payment = _mapper.Map<Payment>(paymentDto);
                var createdPayment = await _paymentService.CreatePaymentAsync(payment);
                var responseDto = _mapper.Map<PaymentResponseDto>(createdPayment);

                return CreatedAtAction(nameof(GetPaymentById), new { id = createdPayment.Id }, responseDto);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when creating payment");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Order not found for payment");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment");
                return BadRequest(new { message = "Error creating payment", error = ex.Message });
            }
        }

        /// <summary>
        /// Update a payment
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePayment(int id, PaymentUpdateDto paymentDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("Updating payment with ID: {PaymentId}", id);
                var payment = _mapper.Map<Payment>(paymentDto);
                var result = await _paymentService.UpdatePaymentAsync(id, payment);

                if (!result)
                {
                    _logger.LogWarning("Payment with ID: {PaymentId} not found for update", id);
                    return NotFound(new { message = $"Payment with ID {id} not found" });
                }

                return Ok(new { message = "Payment updated successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when updating payment");
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Payment not found for update");
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment");
                return BadRequest(new { message = "Error updating payment", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a payment
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePayment(int id)
        {
            try
            {
                _logger.LogInformation("Deleting payment with ID: {PaymentId}", id);
                var result = await _paymentService.DeletePaymentAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Payment with ID: {PaymentId} not found for deletion", id);
                    return NotFound(new { message = $"Payment with ID {id} not found" });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Argument validation error when deleting payment");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment");
                return BadRequest(new { message = "Error deleting payment", error = ex.Message });
            }
        }
    }
}
