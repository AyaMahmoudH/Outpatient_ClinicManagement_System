using HospitalManagement.DTOs;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotificationController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("user")]
        public async Task<IActionResult> SendToUser([FromBody] UserNotificationRequest request)
        {
            await _notificationService.SendNotificationToUser(request.UserId, request.Message);
            return Ok("Notification sent to user.");
        }

        [HttpPost("group")]
        public async Task<IActionResult> SendToGroup([FromBody] GroupNotificationRequest request)
        {
            await _notificationService.SendNotificationToGroup(request.GroupName, request.Message);
            return Ok("Notification sent to group.");
        }

        [HttpPost("all")]
        public async Task<IActionResult> SendToAll([FromBody] BroadcastNotificationRequest request)
        {
            await _notificationService.SendNotificationToAll(request.Message);
            return Ok("Notification sent to all users.");
        }
    }
}
