using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Helpers;
using EmployeeManagementSystem.BusinessLogic.Results;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using EmployeeManagementSystem.DataAccess.Results;

namespace EmployeeManagementSystem.BusinessLogic.Services.Implementations
{
    public class AuthenticationService(IGenericRepository<User> genericUserRepository, IUsersRepository usersRepository,JwtTokenGeneratorHelper jwtTokenGeneratorHelper) : IAuthenticationService
    {
        private readonly IGenericRepository<User> _genericUserRepository = genericUserRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly JwtTokenGeneratorHelper _jwtTokenGeneratorHelper = jwtTokenGeneratorHelper;

        public async Task<ServiceResult<UserRegistrationDto>> RegisterUser(UserRegistrationDto userRegistrationDto)
        {
            try
            {
                User? existingUser = await _usersRepository.GetUser(userRegistrationDto.Email);
                if (existingUser != null)
                {
                    return ServiceResult<UserRegistrationDto>.BadRequest("User with this email already exists.");
                }

                var user = new User
                {
                    Email = userRegistrationDto.Email,
                    Password = userRegistrationDto.Password,
                    FirstName = userRegistrationDto.FirstName,
                    LastName = userRegistrationDto.LastName,
                    UserName = userRegistrationDto.UserName,
                    Address = userRegistrationDto.Address,
                    Zipcode = userRegistrationDto.Zipcode,
                    MobileNo = userRegistrationDto.MobileNo,
                    CountryId = userRegistrationDto.CountryId,
                    StateId = userRegistrationDto.StateId,
                    CityId = userRegistrationDto.CityId,
                    CreatedAt = DateTime.Now,
                    RoleId=userRegistrationDto.RoleId
                };

                RepositoryResult<User> result = await _genericUserRepository.AddAsync(user);

                if (!result.Success)
                {
                    return ServiceResult<UserRegistrationDto>.InternalError($"Failed to register user: {result.ErrorMessage}");
                }

                return ServiceResult<UserRegistrationDto>.Created(userRegistrationDto,"User registered successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<UserRegistrationDto>.InternalError("An unexpected error occurred during registration.", ex);
            }
        }

        public async Task<ServiceResult<UserLoginDto>> Login(UserLoginDto userLoginDto)
        {
            try
            {
                User? existingUser = await _usersRepository.GetUser(userLoginDto.Email);
                if (existingUser == null)
                {
                    return ServiceResult<UserLoginDto>.NotFound("User with this email does not exist.");
                }

                if (existingUser.Password != userLoginDto.Password)
                {
                    return ServiceResult<UserLoginDto>.BadRequest("You have entered an incorrect password.");
                }
                string jwtToken=_jwtTokenGeneratorHelper.GenerateJWT(existingUser);
                userLoginDto.JwtToken=jwtToken;
                return ServiceResult<UserLoginDto>.Ok(userLoginDto,"You have successfully logged in");
            }
            catch (Exception ex)
            {
                return ServiceResult<UserLoginDto>.InternalError("An unexpected error occurred during login.", ex);
            }
        }

    }
}
