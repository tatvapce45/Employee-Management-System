using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Results;

namespace EmployeeManagementSystem.BusinessLogic.Services.Interfaces
{
    public interface IEmployeesService
    {
        Task<ServiceResult<EmployeesResponseDto>> GetEmployees(EmployeesRequestDto employeesRequestDto);

        Task<ServiceResult<EmployeeDto>> GetEmployee(int employeeId);

        Task<ServiceResult<EmployeeDto>> AddEmployee(CreateEmployeeDto createEmployeeDto);

        Task<ServiceResult<EmployeeDto>> UpdateEmployee(UpdateEmployeeDto updateEmployeeDto);

        Task<ServiceResult<EmployeeDto>> DeleteEmployee(int employeeId);

        Task<ServiceResult<string>> DeleteMultipleEmployees(int[] employeeIds);

        Task<ServiceResult<object>> GenerateEmployeesReport(int departnemtId,string? fromDate, string? toDate,string? gender,int? age);

        Task<ServiceResult<byte[]>> GenerateEmployeesReportExcel(int departmentId, string? fromDate, string? toDate, string? gender, int? age);

        Task<ServiceResult<DepartmentsDto>> GetDepartments();

        Task<ServiceResult<DepartmrentDto>> GetDepartment(int departmentId);

        Task<ServiceResult<DepartmrentDto>> AddDepartment(CreateDepartmentDto createDepartmentDto);

        Task<ServiceResult<DepartmrentDto>> UpdateDepartment(UpdateDepartmentDto updateDepartmentDto);

        Task<ServiceResult<DepartmrentDto>> DeleteDepartment(int departmentId);

        Task<ServiceResult<UpdateProfileDto>> GetEmployeeForMyProfile(int employeeId);

        Task<ServiceResult<EmployeeDto>> UpdateProfile(UpdateProfileDto updateProfileDto);

        Task<ServiceResult<ChangePasswordDto>> ChangePassword(ChangePasswordDto changePasswordDto);
    }
}