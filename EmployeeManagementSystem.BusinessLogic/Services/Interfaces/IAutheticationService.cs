using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Results;

namespace EmployeeManagementSystem.BusinessLogic.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<UserRegistrationDto>> RegisterUser(UserRegistrationDto userRegistrationDto);

        Task<ServiceResult<UserLoginDto>> Login(UserLoginDto userLoginDto);
    }
}