using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Results;

namespace EmployeeManagementSystem.BusinessLogic.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<UserRegistrationDto>> RegisterUser(UserRegistrationDto userRegistrationDto);

        Task<ServiceResult<string>> Login(UserLoginDto userLoginDto);

        Task<ServiceResult<TokensDto>> VerifyOtp(string email, string submittedOtp);
    }
}