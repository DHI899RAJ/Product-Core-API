using System.Diagnostics;
using ProductManagementAPI.Interfaces;
using ProductManagementAPI.Models;

namespace ProductManagementAPI.Services.DependencyInjection
{
    /// <summary>
    /// Singleton request logging service using repository pattern.
    /// Provides centralized request timing and audit logging across the application.
    /// </summary>
    public interface IRequestLoggingService
    {
        Task LogRequestAsync(string method, string path, int? statusCode, long elapsedMilliseconds);
        Task<IEnumerable<RequestLog>> GetRequestLogsAsync();
        Task<RequestLog?> GetRequestLogByIdAsync(int id);
    }

    /// <summary>
    /// Implementation of request logging service.
    /// Uses repository pattern for data persistence and access.
    /// </summary>
    public class RequestLoggingService : IRequestLoggingService
    {
        private readonly IRepository<RequestLog> _repository;

        public RequestLoggingService(IRepository<RequestLog> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Log a request with timing information
        /// </summary>
        public async Task LogRequestAsync(string method, string path, int? statusCode, long elapsedMilliseconds)
        {
            var requestLog = new RequestLog
            {
                RequestMethod = method,
                RequestPath = path,
                StatusCode = statusCode,
                ElapsedMilliseconds = elapsedMilliseconds,
                RequestedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(requestLog);
        }

        /// <summary>
        /// Retrieve all request logs
        /// </summary>
        public async Task<IEnumerable<RequestLog>> GetRequestLogsAsync()
        {
            return await _repository.GetAllAsync();
        }

        /// <summary>
        /// Retrieve a specific request log by ID
        /// </summary>
        public async Task<RequestLog?> GetRequestLogByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }
    }
}


