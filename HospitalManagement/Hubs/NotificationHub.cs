using HospitalManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace HospitalManagementSystem.Hubs;

[Authorize]
public class NotificationHub : Hub
{
     public async Task SendNotificationToUser(string userId, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", message);
    }

    public async Task SendNotificationToRole(string role, string message)
    {
        await Clients.Group(role).SendAsync("ReceiveNotification", message);
    }
    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userManager = httpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

        var user = await userManager.GetUserAsync(httpContext.User);
        if (user != null)
        {
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, role);
            }
        }

        await base.OnConnectedAsync();
    }
}