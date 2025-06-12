using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Services.Implementations;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using Google.Apis.Auth;
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
        [ProducesResponseType(typeof(UserRegistrationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto dto)
        {
            var result = await _authenticationService.RegisterUser(dto);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, new { result.Message, result.Success });
            }
            return StatusCode(result.StatusCode, new { result.Message, result.ValidationErrors, result.Success });
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(UserLoginDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
        {
            var result = await _authenticationService.Login(userLoginDto);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, new { result.Message, result.Success });
            }
            return StatusCode(result.StatusCode, new { result.Message, result.ValidationErrors, result.Success });
        }

        [HttpPost("Varify-OTP")]
        [ProducesResponseType(typeof(UserLoginDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyOtp(string email, string otp)
        {
            var result = await _authenticationService.VerifyOtp(email, otp);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, new { result.Data, result.Message });
            }
            return StatusCode(result.StatusCode, new { result.Message, result.ValidationErrors });
        }

        [HttpPost("Refresh-Token")]
        public async Task<IActionResult> Refresh([FromBody] TokenRefreshRequestDto tokenRefreshRequestDto)
        {
            var newAccessToken = await _tokenService.RefreshAccessToken(tokenRefreshRequestDto.RefreshToken);

            if (newAccessToken == null)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }
            return Ok(new { AccessToken = newAccessToken });
        }
    }
}