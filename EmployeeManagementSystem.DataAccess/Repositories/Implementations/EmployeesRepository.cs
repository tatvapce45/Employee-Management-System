using System.Linq.Expressions;
using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace EmployeeManagementSystem.DataAccess.Repositories.Implementations
{
    public class EmployeesRepository(EmployeeManagementSystemContext context, IGenericRepository<Employee> genericRepository) : IEmployeesRepository
    {
        private readonly EmployeeManagementSystemContext _context = context;
        private readonly IGenericRepository<Employee> _genericRepository = genericRepository;

        public async Task<List<Employee>> GetEmployeesAsync(int departmentId, int pageNumber, int pageSize, string sortBy, string sortOrder, string searchTerm)
        {
            Expression<Func<Employee, bool>> filter = e => e.DepartmentId == departmentId;
            return await _genericRepository.GetAsync(pageNumber, pageSize, sortBy, sortOrder, searchTerm, filter, e => e.Name, e => e.Department.Name, e => e.Id.ToString());
        }

        public async Task<bool> CheckIfExists(string email,string mobileNo)
        {
            return await _context.Employees.AnyAsync(e=>e.Email==email || e.MobileNo==mobileNo);
        }

        public async Task<bool> CheckIfExistsWithDifferentId(int employeeId,string email,string mobileNo)
        {
            return await _context.Employees.AnyAsync(e =>e.Id!=employeeId && (e.Email==email || e.MobileNo==mobileNo));
        }
    }
}