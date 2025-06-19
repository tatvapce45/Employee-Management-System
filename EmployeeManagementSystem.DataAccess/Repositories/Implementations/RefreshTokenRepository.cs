using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.DataAccess.Repositories.Implementations
{
    public class RefreshTokenRepository(EmployeeManagementSystemContext context) : IRefreshTokenRepository
    {
        private readonly EmployeeManagementSystemContext _context = context;

        public async Task<Refreshtoken?> GetRefreshtokenByToken(string token)
        {
            return await _context.Refreshtokens
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.Token == token);
        }
    }
}