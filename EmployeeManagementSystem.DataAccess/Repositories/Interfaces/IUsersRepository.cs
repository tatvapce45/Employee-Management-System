using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<User?> GetUser(string email);
    }
}