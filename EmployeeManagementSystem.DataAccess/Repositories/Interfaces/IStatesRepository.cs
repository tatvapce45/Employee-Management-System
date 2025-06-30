using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface IStatesRepository
    {
        Task<List<State1>> GetStatesbyCountryId(int countryId);
    }
}