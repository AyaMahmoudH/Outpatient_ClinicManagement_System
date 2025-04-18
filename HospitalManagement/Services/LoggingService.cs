using HospitalManagement.DTOs;
using HospitalManagement.Models;
using Microsoft.Extensions.Logging;

namespace HospitalManagement.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly List<ActivityLog> _activityLogs = new(); // مؤقت - بدل قاعدة البيانات

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

            _activityLogs.Add(log); // في الواقع تكتبها في قاعدة بيانات
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

        // نسخة من الدالة اللي فيها object parameters – ممكن ترمي Exception أو تتجاهلها
        public Task LogActivityAsync(object value1, object value2, object value3, string v1, string v2, object value4, string v3, string v4)
        {
            throw new NotImplementedException("Use the strongly typed version of LogActivityAsync.");
        }
    }
}
