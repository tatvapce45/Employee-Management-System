using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.BusinessLogic.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.BusinessLogic.Services.Implementations;

namespace EmployeeManagementSystem.Web.Controllers
{
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController(IEmployeesService employeesService, NotificationService notificationService) : ControllerBase
    {
        private readonly IEmployeesService _employeeService = employeesService;
        private readonly NotificationService _notificationService = notificationService;

        [HttpPost("GetEmployees")]
        [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<List<EmployeeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmployees([FromBody] EmployeesRequestDto employeesRequestDto)
        {
            var result = await _employeeService.GetEmployees(employeesRequestDto);
            var response = new ApiCommonResponse<EmployeesResponseDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetEmployeeById")]
        // [Authorize]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmployee(int employeeId)
        {
            var result = await _employeeService.GetEmployee(employeeId);
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

        [HttpPost("CreateEmployee")]
        [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            var result = await _employeeService.AddEmployee(createEmployeeDto);
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

        [HttpPatch("UpdateEmployee")]
        // [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeDto updateEmployeeDto)
        {
            var result = await _employeeService.UpdateEmployee(updateEmployeeDto);
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

        [HttpDelete("DeleteEmployee")]
        [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            var result = await _employeeService.DeleteEmployee(employeeId);
            var response = new ApiCommonResponse<string>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? "Employee deleted successfully." : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GenerateReport")]
        // [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateEmployeeReportExcel(int departmentId, string? fromDate, string? toDate, string? gender, int? age)
        {
            var result = await _employeeService.GenerateEmployeesReportExcel(departmentId, fromDate, toDate, gender, age);

            if (result.Success)
            {
                var fileName = $"EmployeesReport_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(result.Data!, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }

            var response = new ApiCommonResponse<object>
            {
                Success = false,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetDepartments")]
        // [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmentsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartments()
        {
            var result = await _employeeService.GetDepartments();
            var response = new ApiCommonResponse<DepartmentsDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("GetDepartmentById")]
        // [Authorize]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDepartment([FromQuery] int departmentId)
        {
            var result = await _employeeService.GetDepartment(departmentId);
            var response = new ApiCommonResponse<DepartmrentDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("CreateDepartment")]
        // [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddDepartment([FromBody] CreateDepartmentDto createDepartmentDto)
        {
            var result = await _employeeService.AddDepartment(createDepartmentDto);
            var response = new ApiCommonResponse<DepartmrentDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpPatch("UpdateDepartment")]
        // [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDepartment([FromBody] UpdateDepartmentDto updateDepartmentDto)
        {
            var result = await _employeeService.UpdateDepartment(updateDepartmentDto);
            var response = new ApiCommonResponse<DepartmrentDto>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? result.Data : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("DeleteDepartment")]
        // [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteDepartment(int departmentId)
        {
            var result = await _employeeService.DeleteDepartment(departmentId);
            var response = new ApiCommonResponse<string>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = result.Success ? "Department deleted successfully." : null,
                ValidationErrors = result.ValidationErrors
            };

            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("DeleteMultipleEmployees")]
        public async Task<IActionResult> DeleteMultipleEmployees([FromBody] int[] employeeIds)
        {
            var result = await _employeeService.DeleteMultipleEmployees(employeeIds);
            var response = new ApiCommonResponse<string>
            {
                Success = result.Success,
                StatusCode = result.StatusCode,
                Message = result.Message!,
                Data = null
            };
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("send-to-user")]
        public async Task<IActionResult> SendToUser([FromBody] NotificationRequest request)
        {
            var result = await _notificationService.SendNotificationToUserAsync(request.Email, request.Message);

            var response = new ApiCommonResponse<string>
            {
                Success = result,
                StatusCode = 200,
                Message = "notification sent",
                Data = null
            };
            return StatusCode(response.StatusCode, response);
        }
    }
}
