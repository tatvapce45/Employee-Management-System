using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Results;

namespace EmployeeManagementSystem.BusinessLogic.Services.Interfaces
{
    public interface IHomeService
    {
        Task<ServiceResult<DashboardResponseDto>> GetDashboardData(int timeId,string fromDate,string toDate);
    }
}