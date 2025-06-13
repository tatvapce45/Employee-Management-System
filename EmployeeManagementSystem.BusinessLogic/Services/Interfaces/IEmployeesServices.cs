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

        Task<ServiceResult<object>> GenerateEmployeesReport(int departnemtId,string? fromDate, string? toDate,string? gender,int? age);

        Task<ServiceResult<byte[]>> GenerateEmployeesReportExcel(int departmentId, string? fromDate, string? toDate, string? gender, int? age);

        Task<ServiceResult<DepartmentsDto>> GetDepartments();
    }
}