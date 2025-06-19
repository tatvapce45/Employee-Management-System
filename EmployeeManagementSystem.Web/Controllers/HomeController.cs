using EmployeeManagementSystem.BusinessLogic.Common;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Services.Implementations;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController(IEmployeesService employeeService) : ControllerBase
    {
        private readonly IEmployeesService _employeeService=employeeService;

        [HttpGet("GetEmployeeToUpdateProfile")]
        // [Authorize]
        [ProducesResponseType(typeof(ApiCommonResponse<UpdateProfileDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmployee(int employeeId)
        {
            var result = await _employeeService.GetEmployeeForMyProfile(employeeId);
            var response = new ApiCommonResponse<UpdateProfileDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("UpdateProfile")]
        // [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            var result = await _employeeService.UpdateProfile(updateProfileDto);
            var response = new ApiCommonResponse<EmployeeDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("ChangePassword")]
        // [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await _employeeService.ChangePassword(changePasswordDto);
            var response = new ApiCommonResponse<ChangePasswordDto>
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