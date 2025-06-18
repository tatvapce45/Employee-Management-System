using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Services.Implementations;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.BusinessLogic.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
            var result = await _tokenService.RefreshAccessToken(tokenRefreshRequestDto.RefreshToken);

            TokenRefreshResponseDto? tokenRefreshResponseDto = null;

            if (result != null && result.Success && result.Data is not null)
            {
                TokenRefreshResponseDto tokenData = result.Data;

                tokenRefreshResponseDto = new TokenRefreshResponseDto
                {
                    AccessToken = tokenData.AccessToken,
                    RefreshToken = tokenData.RefreshToken
                };
            }

            var response = new ApiCommonResponse<TokenRefreshResponseDto>
            {
                Success = tokenRefreshResponseDto != null,
                StatusCode = tokenRefreshResponseDto != null ? StatusCodes.Status200OK : StatusCodes.Status401Unauthorized,
                Message = tokenRefreshResponseDto != null ? "Token refreshed successfully." : "Invalid or expired refresh token.",
                Data = tokenRefreshResponseDto,
                ValidationErrors = tokenRefreshResponseDto == null ? ["Invalid or expired refresh token."] : null
            };

            return StatusCode(response.StatusCode, response);
        }


        [Authorize]
        [HttpGet("Validate-Token")]
        public IActionResult ValidateToken()
        {
            return Ok(new { Success = true });
        }

        [HttpGet("GetRoles")]
        public async Task<IActionResult> GetRoles()
        {
            var result = await _authenticationService.GetRoles();
            var response = new ApiCommonResponse<RolesResponseDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetCountries")]
        public async Task<IActionResult> GetCountries()
        {
            var result = await _authenticationService.GetCountries();
            var response = new ApiCommonResponse<CountriesResponseDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetStatesByCountryId")]
        public async Task<IActionResult> GetStatesByCountryId([FromQuery]int countryId)
        {
            var result = await _authenticationService.GetStatesByCountryId(countryId);
            var response = new ApiCommonResponse<StatesResponseDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetCitiesByStateId")]
        public async Task<IActionResult> GetCitiesByStateId(int stateId)
        {
            var result = await _authenticationService.GetCitiesByStateId(stateId);
            var response = new ApiCommonResponse<CitiesResponseDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }
    }
}
