namespace EmployeeManagementSystem.BusinessLogic.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string body, string htmlBody);
    }
}