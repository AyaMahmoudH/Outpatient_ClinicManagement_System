using HospitalManagement.DTOs;
using HospitalManagement.Models;
using Microsoft.Extensions.Logging;

namespace HospitalManagement.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly List<ActivityLog> _activityLogs = new(); 
        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public async Task LogActivityAsync(string userId, string userName, string userRole, string action, string entityName, string entityId, string details, string ipAddress)
        {
            var log = new ActivityLog
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                UserName = userName,
                UserRole = userRole,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            _activityLogs.Add(log); 
            _logger.LogInformation($"User {userName} performed {action} on {entityName} ({entityId}) from {ipAddress}");

            await Task.CompletedTask;
        }

        public async Task<IEnumerable<ActivityLog>> GetActivityLogsAsync(DateTime startDate, DateTime endDate)
        {
            var result = _activityLogs.Where(log => log.Timestamp >= startDate && log.Timestamp <= endDate);
            return await Task.FromResult(result);
        }

        public async Task<IEnumerable<ActivityLog>> GetUserActivityLogsAsync(string userId)
        {
            var result = _activityLogs.Where(log => log.UserId == userId);
            return await Task.FromResult(result);
        }

        
    }
}
