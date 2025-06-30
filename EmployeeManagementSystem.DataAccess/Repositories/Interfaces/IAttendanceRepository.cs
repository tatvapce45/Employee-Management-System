using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<bool> IsAttendanceFilled(DateOnly date,int employeeId);

        Task<Attendance?> GetAttendanceByEmployeeAndDate(DateOnly date, int employeeId);

        Task<List<Attendance>> GetAttendanceListByMonthYearEmployeeId(int employeeId,int year,int month);

        Task<List<Attendance>> GetRecordsWithNullStatusForDate(DateOnly date);
    }
}