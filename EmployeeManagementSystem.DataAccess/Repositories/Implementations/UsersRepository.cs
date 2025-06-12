using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace EmployeeManagementSystem.DataAccess.Repositories.Implementations
{
    public class UsersRepository(EmployeeManagementSystemContext context) : IUsersRepository
    {
        private readonly EmployeeManagementSystemContext _context = context;

        public async Task<User?> GetUserByEmail(string email)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(h => h.Email == email);
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(h => h.Id == id);
        }

            public async Task<User?> GetUserByGoogleUserId(string googleUserId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.GoogleUserId == googleUserId);
        }

    }
}