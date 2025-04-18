using System.Threading.Tasks;

namespace HospitalManagement.Services
{
    public interface INotificationService
    {
        Task SendNotificationToUser(string userId, string message);
        Task SendNotificationToGroup(string groupName, string message);
        Task SendNotificationToAll(string message);
        Task CreateNotificationAsync(string userId, string content, string type, string referenceId);
        Task SendRealtimeNotificationAsync(string userId, string message);
    }
}
