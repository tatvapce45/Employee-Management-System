using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Results;

namespace EmployeeManagementSystem.BusinessLogic.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<ServiceResult<string>> MarkAttendance(MarkAttendanceDto markAttendanceDto);

        Task<ServiceResult<string>> UpdateAttendance(UpdateAttendanceDto updateAttendanceDto);

        Task<ServiceResult<AttendanceListDto>> GetAttendanceList(AttendanceListRequestDto attendanceListRequestDto);
    }
}