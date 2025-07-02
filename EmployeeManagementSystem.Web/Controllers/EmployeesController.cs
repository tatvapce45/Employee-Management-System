using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.BusinessLogic.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagementSystem.BusinessLogic.Services.Implementations;

namespace EmployeeManagementSystem.Web.Controllers
{
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(typeof(ApiCommonResponse<List<EmployeeDto>>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiCommonResponse<List<EmployeeDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiCommonResponse<List<EmployeeDto>>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Retrieves a paginated, sorted, and filtered list of employees based on the provided criteria.
        /// </summary>
        /// <param name="employeesRequestDto">Parameters for filtering, pagination, sorting, and searching employees.</param>
        /// <returns>
        /// A paged list of employees matching the criteria, or a not found result if none exist.
        /// </returns>
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
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Retrieves an employee's details by their ID.
        /// </summary>
        /// <param name="employeeId">The unique identifier of the employee.</param>
        /// <returns>
        /// Employee details if found; otherwise, a not found or error response.
        /// </returns>
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
        // [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Creates a new employee record.
        /// </summary>
        /// <param name="createEmployeeDto">The employee details to create.</param>
        /// <returns>
        /// Returns the created employee details if successful; otherwise, returns validation or error information.
        /// </returns>
        public async Task<IActionResult> AddEmployee([FromForm] CreateEmployeeDto createEmployeeDto)
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
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiCommonResponse<EmployeeDto>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Updates an existing employee's details.
        /// </summary>
        /// <param name="updateEmployeeDto">The employee data to update.</param>
        /// <returns>
        /// Returns the updated employee details if successful; otherwise, returns validation or error information.
        /// </returns>
        public async Task<IActionResult> UpdateEmployee([FromForm]UpdateEmployeeDto updateEmployeeDto)
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
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Deletes an employee by their ID.
        /// </summary>
        /// <param name="employeeId">The unique identifier of the employee to delete.</param>
        /// <returns>
        /// Returns a success message if deletion is successful; otherwise, returns not found or error information.
        /// </returns>
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
        [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiCommonResponse<FileResult>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Generates an Excel report of employees based on filtering criteria.
        /// </summary>
        /// <param name="departmentId">The department ID to filter employees.</param>
        /// <param name="fromDate">Optional start date for employee filtering (format: yyyy-MM-dd).</param>
        /// <param name="toDate">Optional end date for employee filtering (format: yyyy-MM-dd).</param>
        /// <param name="gender">Optional gender filter.</param>
        /// <param name="age">Optional age filter.</param>
        /// <returns>
        /// Returns an Excel file (.xlsx) containing the employee report if successful.
        /// Returns an error response if the report generation fails.
        /// </returns>
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
        [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmentsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmentsDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmentsDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmentsDto>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Retrieves a list of all departments.
        /// </summary>
        /// <returns>
        /// Returns a collection of departments wrapped in a DepartmentsDto.
        /// Returns a 404 Not Found response if no departments exist.
        /// </returns>
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
        [Authorize]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Retrieves the details of a department by its ID.
        /// </summary>
        /// <param name="departmentId">The unique identifier of the department.</param>
        /// <returns>
        /// Returns the department details wrapped in a DepartmrentDto if found.
        /// Returns 404 Not Found if the department does not exist.
        /// Handles unexpected errors with appropriate error response.
        /// </returns>
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
        [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Creates a new department if the name does not already exist.
        /// </summary>
        /// <param name="createDepartmentDto">The DTO containing details for the new department.</param>
        /// <returns>
        /// Returns the created department data on success with a 201 status code.
        /// Returns 400 Bad Request if a department with the same name already exists.
        /// Handles unexpected errors with a 500 Internal Server Error response.
        /// </returns>
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
        [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Updates an existing department's details.
        /// </summary>
        /// <param name="updateDepartmentDto">DTO containing updated department data including Id.</param>
        /// <returns>
        /// Returns the updated department data with a 200 OK status on success.
        /// Returns 400 Bad Request if a department with the same name exists but has a different Id,
        /// or if the department to update does not exist.
        /// Returns 500 Internal Server Error for unexpected failures.
        /// </returns>
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
        [Authorize(Roles = "HR Manager,Admin")]
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiCommonResponse<string>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Deletes a department by its ID, including all employees under that department.
        /// </summary>
        /// <param name="departmentId">The ID of the department to delete.</param>
        /// <returns>
        /// Returns 200 OK with a success message if deletion is successful.
        /// Returns 404 Not Found if the department does not exist.
        /// Returns 500 Internal Server Error for unexpected failures.
        /// </returns>
        /// <remarks>
        /// The method first retrieves the department. If found, it fetches all employees in that department and deletes them individually before deleting the department itself.
        /// </remarks>
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
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status206PartialContent)]
        [ProducesResponseType(typeof(ApiCommonResponse<DepartmrentDto>), StatusCodes.Status500InternalServerError)]
        /// <summary>
        /// Deletes multiple employees by their IDs.
        /// </summary>
        /// <param name="employeeIds">Array of employee IDs to delete.</param>
        /// <returns>
        /// Returns 200 OK if all employees are deleted successfully.
        /// Returns 206 Partial Content if some employees failed to delete, listing their IDs.
        /// Returns 400 Bad Request if no IDs are provided.
        /// Returns 500 Internal Server Error for unexpected failures.
        /// </returns>
        /// <remarks>
        /// The method attempts to delete each employee by ID. If an employee is not found or deletion fails, the ID is recorded and reported in the response.
        /// </remarks>
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
        /// <summary>
        /// Sends a real-time notification message to a specific user identified by their email.
        /// </summary>
        /// <param name="request">The notification request containing the user's email and the message to send.</param>
        /// <returns>
        /// Returns 200 OK with a success message if the notification is sent successfully.
        /// Returns 500 Internal Server Error if sending the notification fails.
        /// </returns>
        /// <remarks>
        /// The method uses SignalR to send the notification to the user's group based on their email.
        /// Exceptions during sending are caught and handled gracefully, returning a failure response.
        /// </remarks>
        public async Task<IActionResult> SendToUser([FromBody] NotificationRequest request)
        {
            var isSent = await _notificationService.SendNotificationToUserAsync(request.Email, request.Message);

            if (isSent)
            {
                var response = new ApiCommonResponse<string>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Notification sent successfully.",
                    Data = null
                };
                return Ok(response);
            }
            else
            {
                var response = new ApiCommonResponse<string>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Failed to send notification.",
                    Data = null
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

    }
}
