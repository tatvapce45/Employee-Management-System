using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController(IEmployeesService employeesService) : ControllerBase
    {
        private readonly IEmployeesService _employeeService = employeesService;

        [Authorize(Roles = "HR Manager,Admin")]
        [HttpPost("Employees")]
        [ProducesResponseType(typeof(EmployeesResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetEmployees([FromBody] EmployeesRequestDto employeesRequestDto)
        {
            var result = await _employeeService.GetEmployees(employeesRequestDto);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result.Data);
            }
            return StatusCode(result.StatusCode, new
            {
                result.Message,
                result.ValidationErrors,
                ExceptionMessage = result.Exception?.Message,
                ExceptionStackTrace = result.Exception?.StackTrace
            });
        }

        [HttpGet("Employee")]
        public async Task<IActionResult> GetEmployee(int employeeId)
        {
            var result = await _employeeService.GetEmployee(employeeId);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, result.Data);
            }
            return StatusCode(result.StatusCode, new
            {
                result.Message,
                result.ValidationErrors,
                ExceptionMessage = result.Exception?.Message,
                ExceptionStackTrace = result.Exception?.StackTrace
            });
        }

        [HttpPost("Employee")]
        public async Task<IActionResult> AddEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            var result = await _employeeService.AddEmployee(createEmployeeDto);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, new { result.Data, result.Message });
            }
            return StatusCode(result.StatusCode, new
            {
                result.Message,
                result.ValidationErrors,
                ExceptionMessage = result.Exception?.Message,
                ExceptionStackTrace = result.Exception?.StackTrace
            });
        }

        [HttpPatch("Employee")]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            var result = await _employeeService.UpdateEmployee(updateEmployeeDto);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, new { result.Data, result.Message });
            }
            return StatusCode(result.StatusCode, new
            {
                result.Message,
                result.ValidationErrors,
                ExceptionMessage = result.Exception?.Message,
                ExceptionStackTrace = result.Exception?.StackTrace
            });
        }

        [HttpDelete("Employee")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            var result = await _employeeService.DeleteEmployee(employeeId);
            if (result.Success)
            {
                return StatusCode(result.StatusCode, new { result.Message });
            }
            return StatusCode(result.StatusCode, new
            {
                result.Message,
                result.ValidationErrors,
                ExceptionMessage = result.Exception?.Message,
                ExceptionStackTrace = result.Exception?.StackTrace
            });
        }
    }
}