using EmployeeManagementSystem.BusinessLogic.Helpers;
using EmployeeManagementSystem.BusinessLogic.Results;
using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;

namespace EmployeeManagementSystem.BusinessLogic.Services.Implementations
{
    public class TokenService(JwtTokenGeneratorHelper jwtHelper, IGenericRepository<Refreshtoken> refreshTokenGenericRepository, IRefreshTokenRepository refreshTokenRepository,IUsersRepository usersRepository)
    {
        private readonly JwtTokenGeneratorHelper _jwtHelper = jwtHelper;
        private readonly IGenericRepository<Refreshtoken> _refreshTokenGenericRepository = refreshTokenGenericRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;

        public async Task<ServiceResult<object>> GenerateTokensAsync(User user)
        {
            var accessToken = _jwtHelper.GenerateJWT(user);

            var refreshToken = new Refreshtoken
            {
                Token = Guid.NewGuid().ToString(),
                Expires = DateTime.Now.AddDays(7),
                UserId = user.Id,
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

        public async Task<string?> RefreshAccessToken(string refreshToken)
        {
            var token = await _refreshTokenRepository.GetRefreshtokenByToken(refreshToken);

            if (token == null || token.IsUsed || token.IsRevoked || token.Expires < DateTime.Now)
            {
                return null;
            }

            token.IsUsed = true;
            var result = await _refreshTokenGenericRepository.UpdateAsync(token);
            if (!result.Success)
            {
                throw new Exception($"Failed to update refresh token: {result.ErrorMessage}");
            }
            User? user=await _usersRepository.GetUserByEmail(token.User.Email);
            if(user!=null)
            {
                token.User = user;
            }
            else
            {
                throw new Exception("User not found for the provided refresh token.");
            }
            return _jwtHelper.GenerateJWT(token.User);
        }
    }
}
