using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Helpers;
using EmployeeManagementSystem.BusinessLogic.Results;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;

namespace EmployeeManagementSystem.BusinessLogic.Services.Implementations
{
    public class TokenService:ITokenService
    {
        private readonly IJwtTokenGeneratorHelper _jwtHelper;
        private readonly IGenericRepository<Refreshtoken> _refreshTokenGenericRepository ;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IEmployeesRepository _employeesRepository;
        public TokenService(IJwtTokenGeneratorHelper jwtHelper, IGenericRepository<Refreshtoken> refreshTokenGenericRepository, IRefreshTokenRepository refreshTokenRepository, IEmployeesRepository employeesRepository)
        {
            _jwtHelper=jwtHelper;
            _refreshTokenGenericRepository=refreshTokenGenericRepository;
            _refreshTokenRepository=refreshTokenRepository;
            _employeesRepository=employeesRepository;
        }

        public async Task<ServiceResult<object>> GenerateTokens(Employee employee)
        {
            var accessToken = _jwtHelper.GenerateJWT(employee);

            var refreshToken = new Refreshtoken
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.Now.AddDays(7),
                EmployeeId = employee.Id,
                IsUsed = false,
                IsRevoked = false
            };

            var result = await _refreshTokenGenericRepository.AddAsync(refreshToken);
            if (!result.Success)
            {
                throw new Exception($"Failed to create refresh token: {result.ErrorMessage}");
            }
            return new ServiceResult<object>
            {
                Success = true,
                Data = new { AccessToken = accessToken, RefreshToken = refreshToken.Token }
            };
        }

        public async Task<ServiceResult<TokenRefreshResponseDto>?> RefreshAccessToken(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetRefreshtokenByToken(refreshToken);

            if (token == null || token.IsUsed || token.IsRevoked || token.Expires < DateTime.Now)
                return null;

            token.IsUsed = true;
            await _refreshTokenGenericRepository.UpdateAsync(token);

            var employee = await _employeesRepository.GetEmployeeByEmail(token.Employee.Email);
            if (employee == null)
            {
                return null;
            }

            var newAccessToken = _jwtHelper.GenerateJWT(employee);

            var newRefreshToken = new Refreshtoken
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.Now.AddDays(7),
                EmployeeId = employee.Id,
                IsUsed = false,
                IsRevoked = false
            };

            await _refreshTokenGenericRepository.AddAsync(newRefreshToken);

            return new ServiceResult<TokenRefreshResponseDto>
            {
                Success = true,
                Data = new TokenRefreshResponseDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken.Token }
            };
        }

    }
}
