using EmployeeManagementSystem.BusinessLogic.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Build.Framework;

namespace EmployeeManagementSystem.BusinessLogic.Services.Implementations
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string message);
    }

    public class NotificationService(IHubContext<NotificationHub> hubContext) : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext = hubContext;

        public Task SendNotificationAsync(string message)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendNotificationToUserAsync(string email, string message)
        {
            try
            {
                await _hubContext.Clients.Group(email).SendAsync("ReceiveNotification", message);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
