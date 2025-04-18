using HospitalManagement.Data;
using HospitalManagement.Models;
using HospitalManagementSystem.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HospitalManagement.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ApplicationDbContext _context;

        public NotificationService(IHubContext<NotificationHub> hubContext, ApplicationDbContext context)
        {
            _hubContext = hubContext;
            _context = context;
        }
        public async Task SendNotificationToUser(string userId, string message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
        public async Task SendNotificationToGroup(string groupName, string message)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", message);
        }
        public async Task SendNotificationToAll(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }
        public async Task CreateNotificationAsync(string userId, string content, string type, string referenceId)
        {
            var notification = new Notification
            {
                UserId = userId,
                Content = content,
                Type = type,
                ReferenceId = referenceId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task SendRealtimeNotificationAsync(string userId, string message)
        {
            await SendNotificationToUser(userId, message);
        }
    }
}
