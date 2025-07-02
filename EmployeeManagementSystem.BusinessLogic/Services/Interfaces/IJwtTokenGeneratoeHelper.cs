using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.BusinessLogic.Services.Interfaces
{
    public interface IJwtTokenGeneratorHelper
    {
        string GenerateJWT(Employee employee, int expireMinutes = 15);
    }
}