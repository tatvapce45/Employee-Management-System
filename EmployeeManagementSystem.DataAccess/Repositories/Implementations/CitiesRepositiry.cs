using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.DataAccess.Repositories.Implementations
{
    public class CitiesRepository(EmployeeManagementSystemContext context) : ICitiesRepository
    {
        private readonly EmployeeManagementSystemContext _context=context;
        public async Task<List<City>> GetCitiesByStateId(int stateId)
        {
            return await _context.Cities.Where(c=>c.StateId==stateId).ToListAsync();
        }
    }
}