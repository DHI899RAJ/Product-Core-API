using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.Models;
using ProductManagementAPI.Services.DependencyInjection;

namespace ProductManagementAPI.Controllers
{
    /// <summary>
    /// Controller for managing request logs
    /// Provides endpoints to retrieve audit logs of API requests and their timing information
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RequestLogsController : ControllerBase
    {
        private readonly IRequestLoggingService _loggingService;
        private readonly ILogger<RequestLogsController> _logger;

        public RequestLogsController(IRequestLoggingService loggingService, ILogger<RequestLogsController> logger)
        {
            _loggingService = loggingService;
            _logger = logger;
        }

        /// <summary>
        /// Get all request logs
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RequestLog>>> GetRequestLogs()
        {
            _logger.LogInformation("Retrieving all request logs");
            var logs = await _loggingService.GetRequestLogsAsync();
            return Ok(logs);
        }

        /// <summary>
        /// Get request log by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RequestLog>> GetRequestLogById(int id)
        {
            _logger.LogInformation("Retrieving request log with ID: {LogId}", id);
            var log = await _loggingService.GetRequestLogByIdAsync(id);

            if (log == null)
            {
                _logger.LogWarning("Request log with ID: {LogId} not found", id);
                return NotFound(new ErrorResponse
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Request log not found",
                    TraceId = HttpContext.TraceIdentifier
                });
            }

            return Ok(log);
        }
    }
}

