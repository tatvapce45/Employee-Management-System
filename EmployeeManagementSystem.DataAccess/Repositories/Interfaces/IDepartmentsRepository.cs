namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface IDepartmentsRepository
    {
        Task<bool> CheckIfExists(int id);
    }
}