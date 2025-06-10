using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Web.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService = authenticationService;

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
                return StatusCode(result.StatusCode, result.Data);
            }
            return StatusCode(result.StatusCode, new { result.Message, result.ValidationErrors });
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
                return StatusCode(result.StatusCode,new {result.Data,result.Message});
            }
            return StatusCode(result.StatusCode, new { result.Message, result.ValidationErrors });
        }
    }
}