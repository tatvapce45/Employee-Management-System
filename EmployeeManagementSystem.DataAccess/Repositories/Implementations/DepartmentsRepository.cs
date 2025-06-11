using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.DataAccess.Repositories.Implementations
{
    public class DepartmentsRepository(EmployeeManagementSystemContext context):IDepartmentsRepository
    {
        private readonly EmployeeManagementSystemContext _context = context;
        public async Task<bool> CheckIfExists(int id)
        {
            return await _context.Departments.AnyAsync(d=>d.Id==id);
        }
    }
}