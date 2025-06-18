using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface IRolesRepository
    {
        Task<List<Role>> GetRoles();
    }
}