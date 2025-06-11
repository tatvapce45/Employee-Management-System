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
    }
}