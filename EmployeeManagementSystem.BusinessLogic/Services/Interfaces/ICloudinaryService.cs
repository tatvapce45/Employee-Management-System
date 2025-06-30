using Microsoft.AspNetCore.Http;

namespace EmployeeManagementSystem.BusinessLogic.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<(string Url, string PublicId)> UploadImageAsync(IFormFile file);
    }
}