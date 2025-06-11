using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<Refreshtoken?> GetRefreshtokenByToken(string token);
    }
}