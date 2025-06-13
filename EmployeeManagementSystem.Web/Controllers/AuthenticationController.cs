using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Services.Implementations;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.BusinessLogic.Common;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService, TokenService tokenService) : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;
        private readonly TokenService _tokenService = tokenService;

        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto dto)
        {
            var result = await _authenticationService.RegisterUser(dto);
            var response = new ApiCommonResponse<UserRegistrationDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? dto : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var result = await _authenticationService.Login(userLoginDto);
            var response = new ApiCommonResponse<UserLoginDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? userLoginDto : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("Verify-OTP")]
        public async Task<IActionResult> VerifyOtp([FromQuery] string email, [FromQuery] string otp)
        {
            var result = await _authenticationService.VerifyOtp(email, otp);
            var response = new ApiCommonResponse<TokensDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("Refresh-Token")]
        public async Task<IActionResult> Refresh([FromBody] TokenRefreshRequestDto tokenRefreshRequestDto)
        {
            var newAccessToken = await _tokenService.RefreshAccessToken(tokenRefreshRequestDto.RefreshToken);

            var response = new ApiCommonResponse<object>
            {
                Success = newAccessToken != null,
                StatusCode = newAccessToken != null ? StatusCodes.Status200OK : StatusCodes.Status401Unauthorized,
                Message = newAccessToken != null ? "Token refreshed successfully." : "Invalid or expired refresh token.",
                Data = newAccessToken != null ? new { AccessToken = newAccessToken } : null,
                ValidationErrors = newAccessToken == null ? ["Invalid or expired refresh token."] : null
            };

            return StatusCode(response.StatusCode, response);
        }

    }
}
