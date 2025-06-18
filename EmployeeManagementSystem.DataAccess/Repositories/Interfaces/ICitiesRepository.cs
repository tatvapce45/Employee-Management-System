using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface ICitiesRepository
    {
        Task<List<City>> GetCitiesByStateId(int stateId);
    }
}