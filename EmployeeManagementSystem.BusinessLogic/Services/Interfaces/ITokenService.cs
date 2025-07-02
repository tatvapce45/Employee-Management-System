using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Results;
using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.BusinessLogic.Services.Interfaces
{
    public interface ITokenService
    {
        Task<ServiceResult<object>> GenerateTokens(Employee employee);

        Task<ServiceResult<TokenRefreshResponseDto>?> RefreshAccessToken(string refreshToken);
    }
}