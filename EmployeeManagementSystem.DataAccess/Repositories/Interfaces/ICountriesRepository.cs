using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface ICountriesRepository
    {
        Task<List<Country>> GetCountries();
    }
}