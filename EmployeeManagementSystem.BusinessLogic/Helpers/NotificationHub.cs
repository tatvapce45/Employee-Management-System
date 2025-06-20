using Microsoft.AspNetCore.SignalR;

namespace EmployeeManagementSystem.BusinessLogic.Helpers
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var email = httpContext.Request.Query["email"];

            if (!string.IsNullOrEmpty(email))
            {
                // Add email -> connection mapping
                await Groups.AddToGroupAsync(Context.ConnectionId, email);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var email = httpContext.Request.Query["email"];

            if (!string.IsNullOrEmpty(email))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, email);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
