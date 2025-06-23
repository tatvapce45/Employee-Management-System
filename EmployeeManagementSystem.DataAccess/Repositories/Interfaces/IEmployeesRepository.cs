using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Results;

namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface IEmployeesRepository
    {
        Task<PagedResult<Employee>> GetEmployeesAsync(int departmentId,int pageNumber,int pageSize,string sortBy,string sortOrder,string searchTerm);

        Task<bool> CheckIfExists(string email,string mobileNo);

        Task<bool> CheckIfExistsWithDifferentId(int employeeId,string email,string mobileNo);

        Task<List<Employee>> GetEmployeesForReport(int departmentId,DateOnly? fromDate, DateOnly? toDate,string? gender,int? age);

        Task<Employee?> GetEmployeeByEmail(string email);

        Task<Employee?> GetEmployeeById(int id);

        IQueryable<Employee> GetEmployeesDataForTime(DateTime from,DateTime to);

        IQueryable<Employee> GetAllEmployees();

        Task<List<Employee>> GetEmployeesByDepartmentId(int departmentId);
    }
}