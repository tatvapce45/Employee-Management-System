using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.DataAccess.Repositories.Implementations
{
    public class RolesRepository(EmployeeManagementSystemContext context):IRolesRepository
    {
        private readonly EmployeeManagementSystemContext _context=context;
        public async Task<List<Role>> GetRoles()
        {
            return await _context.Roles.Where(r=>r.Name!="Employee").ToListAsync();
        }
    }
}