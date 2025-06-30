using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.DataAccess.Repositories.Implementations
{
    public class StatesRepository(EmployeeManagementSystemContext context) : IStatesRepository
    {
        private readonly EmployeeManagementSystemContext _context=context;
        public async Task<List<State1>> GetStatesbyCountryId(int countryId)
        {
            return await _context.States1.Where(s=>s.CountryId==countryId).ToListAsync();
        }
    }
}