using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Results;

namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface IDepartmentsRepository
    {
        Task<bool> CheckIfExists(int id);

        Task<List<Department>> GetAllDepartments();

        Task<bool> CheckIfExistsWithName(string name);

        Task<bool> CheckIfExistsWithNameAndDiffId(string name,int id);
    }
}