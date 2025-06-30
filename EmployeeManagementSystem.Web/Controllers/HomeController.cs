using EmployeeManagementSystem.BusinessLogic.Common;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Services.Implementations;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController(IEmployeesService employeeService, IHomeService homeService) : ControllerBase
    {
        private readonly IEmployeesService _employeeService = employeeService;
        private readonly IHomeService _homeService = homeService;

        [HttpGet("GetEmployeeToUpdateProfile")]
        // [Authorize]
        [ProducesResponseType(typeof(ApiCommonResponse<UpdateProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiCommonResponse<UpdateProfileDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiCommonResponse<UpdateProfileDto>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// API endpoint to get employee profile data for updating.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <returns>
        /// HTTP 200 with employee profile on success,
        /// HTTP 404 if employee not found,
        /// HTTP 500 if an internal error occurs.
        /// </returns>
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
        [Authorize]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Updates the profile information of an authenticated employee.
        /// </summary>
        /// <param name="updateProfileDto">The employee profile data to update.</param>
        /// <returns>
        /// Returns 200 OK with updated employee data on success.
        /// Returns 400 Bad Request if validation fails or user is not found.
        /// Returns 500 Internal Server Error if an unexpected error occurs during the update.
        /// </returns>
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto updateProfileDto)
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
        [Authorize]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Allows an authenticated employee to change their password.
        /// </summary>
        /// <param name="changePasswordDto">The current and new password details.</param>
        /// <returns>
        /// Returns 200 OK if the password is changed successfully.
        /// Returns 400 Bad Request if the input is invalid or the change fails.
        /// Returns 500 Internal Server Error if an unexpected error occurs during the process.
        /// </returns>
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

        [HttpGet("GetDashboardData")]
        /// <summary>
        /// Retrieves dashboard data based on the specified time filter and optional date range.
        /// </summary>
        /// <param name="timeId">The time period identifier (default is 1).</param>
        /// <param name="fromDate">Optional start date for filtering data.</param>
        /// <param name="toDate">Optional end date for filtering data.</param>
        /// <returns>
        /// Returns 200 OK with dashboard data.
        /// </returns>
        [ProducesResponseType(typeof(ApiCommonResponse<DashboardResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboardData(int timeId = 1, string fromDate = "", string toDate = "")
        {
            var result = await _homeService.GetDashboardData(timeId, fromDate, toDate);
            var response = new ApiCommonResponse<DashboardResponseDto>
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