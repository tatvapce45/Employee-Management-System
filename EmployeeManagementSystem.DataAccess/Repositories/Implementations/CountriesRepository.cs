using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.DataAccess.Repositories.Implementations
{
    public class CountriesRepository(EmployeeManagementSystemContext context) : ICountriesRepository
    {
        private readonly EmployeeManagementSystemContext _context=context;
        public async Task<List<Country>> GetCountries()
        {
            return await _context.Countries.ToListAsync();
        }
    }
}