using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface IEmployeesRepository
    {
        Task<List<Employee>> GetEmployeesAsync(int departmentId,int pageNumber,int pageSize,string sortBy,string sortOrder,string searchTerm);

        Task<bool> CheckIfExists(string email,string mobileNo);

        Task<bool> CheckIfExistsWithDifferentId(int employeeId,string email,string mobileNo);

        Task<List<Employee>> GetEmployeesForReport(int departmentId,DateOnly? fromDate, DateOnly? toDate,string? gender,int? age);
    }
}