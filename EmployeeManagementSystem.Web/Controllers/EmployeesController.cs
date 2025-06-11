using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagementSystem.Web.Controllers
{
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController(IEmployeesService employeesService) : ControllerBase
    {
        private readonly IEmployeesService _employeeService = employeesService;

        [HttpPost("GetEmployees")]
        [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(EmployeesResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
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

        [HttpGet("GetEmployeeById")]
        [Authorize]
        [ProducesResponseType(typeof(EmployeesResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
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

        [HttpPost("CreateEmployee")]
        [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(EmployeesResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
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

        [HttpPatch("UpdateEmployee")]
        [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(EmployeesResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
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

        [HttpDelete("DeleteEmployee")]
        [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(EmployeesResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
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

        [HttpGet("GenerateReport")]
        [ProducesResponseType(typeof(EmployeesResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GenerateEmployeeReportExcel(int departmentId, string? fromDate, string? toDate, string? gender, int? age)
        {
            var result = await _employeeService.GenerateEmployeesReportExcel(departmentId, fromDate, toDate, gender, age);

            if (result.Success)
            {
                var fileName = $"EmployeesReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(result.Data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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