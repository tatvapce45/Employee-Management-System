using System.Linq.Expressions;
using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using EmployeeManagementSystem.DataAccess.Results;
using Microsoft.EntityFrameworkCore;
namespace EmployeeManagementSystem.DataAccess.Repositories.Implementations
{
    public class EmployeesRepository(EmployeeManagementSystemContext context, IGenericRepository<Employee> genericRepository) : IEmployeesRepository
    {
        private readonly EmployeeManagementSystemContext _context = context;
        private readonly IGenericRepository<Employee> _genericRepository = genericRepository;

        public async Task<PagedResult<Employee>> GetEmployeesAsync(int departmentId, int pageNumber, int pageSize, string sortBy, string sortOrder, string searchTerm)
        {
            Expression<Func<Employee, bool>> filter = e => e.DepartmentId == departmentId;
            return await _genericRepository.GetAsync(pageNumber, pageSize, sortBy, sortOrder, searchTerm, filter, [e => e.Department], e => e.Name, e => e.Department.Name, e => e.Id.ToString());
        }


        public async Task<bool> CheckIfExists(string email, string mobileNo)
        {
            return await _context.Employees.AnyAsync(e => e.Email == email || e.MobileNo == mobileNo);
        }

        public async Task<bool> CheckIfExistsWithDifferentId(int employeeId, string email, string mobileNo)
        {
            return await _context.Employees.AnyAsync(e => e.Id != employeeId && (e.Email == email || e.MobileNo == mobileNo));
        }

        public async Task<List<Employee>> GetEmployeesForReport(int departmentId, DateOnly? fromDate, DateOnly? toDate, string? gender, int? age)
        {
            var from = fromDate?.ToDateTime(TimeOnly.MinValue);
            var to = toDate?.ToDateTime(TimeOnly.MaxValue);

            return await _context.Employees
                .Where(e =>
                    e.DepartmentId == departmentId &&
                    (!fromDate.HasValue || e.HiringDate >= from) &&
                    (!toDate.HasValue || e.HiringDate <= to) &&
                    (string.IsNullOrEmpty(gender) || e.Gender == gender) &&
                    (!age.HasValue || e.Age == age.Value))
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByEmail(string email)
        {
            return await _context.Employees.Include(u => u.Role).FirstOrDefaultAsync(h => h.Email == email);
        }

        public async Task<Employee?> GetEmployeeById(int id)
        {
            return await _context.Employees.Include(u => u.Role).FirstOrDefaultAsync(h => h.Id == id);
        }

        public IQueryable<Employee> GetEmployeesDataForTime(DateTime from, DateTime to)
        {
            return _context.Employees.Where(e => e.HiringDate >= from && e.HiringDate <= to);
        }

        public IQueryable<Employee> GetAllEmployees()
        {
            return _context.Employees.Include(e => e.Department).Include(e=>e.Country);
        }

        public async Task<List<Employee>> GetEmployeesByDepartmentId(int departmentId)
        {
            return await _context.Employees.Where(e => e.DepartmentId == departmentId).ToListAsync();
        }
    }
}