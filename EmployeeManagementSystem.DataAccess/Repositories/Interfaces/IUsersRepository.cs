using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface IUsersRepository
    {
        Task<User?> GetUserByEmail(string email);

        Task<User?> GetUserById(int id);

        Task<User?> GetUserByGoogleUserId(string googleUserId);

    }
}