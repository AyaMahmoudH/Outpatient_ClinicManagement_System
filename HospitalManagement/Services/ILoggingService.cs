using HospitalManagement.DTOs;
using HospitalManagement.Models;
using XAct.Messages;

namespace HospitalManagement.Services
{
    public interface ILoggingService
    {
        Task LogActivityAsync(string userId, string userName, string userRole, string action, string entityName, string entityId, string details, string ipAddress);
        Task<IEnumerable<ActivityLog>> GetActivityLogsAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ActivityLog>> GetUserActivityLogsAsync(string userId);
    }
}
