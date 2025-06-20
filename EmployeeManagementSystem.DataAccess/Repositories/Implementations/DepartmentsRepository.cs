using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.DataAccess.Repositories.Implementations
{
    public class DepartmentsRepository(EmployeeManagementSystemContext context) : IDepartmentsRepository
    {
        private readonly EmployeeManagementSystemContext _context = context;
        public async Task<bool> CheckIfExists(int id)
        {
            return await _context.Departments.AnyAsync(d => d.Id == id);
        }

        public async Task<List<Department>> GetAllDepartments()
        {
            return await _context.Departments.OrderBy(d=>d.CreatedAt).ToListAsync();
        }

        public async Task<bool> CheckIfExistsWithName(string name)
        {
            return await _context.Departments.AnyAsync(d =>d.Name==name);
        }

        public async Task<bool> CheckIfExistsWithNameAndDiffId(string name,int id)
        {
            return await _context.Departments.AnyAsync(d=>d.Name==name && d.Id!=id);
        }
    }
}