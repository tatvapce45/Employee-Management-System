using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.DataAccess.Repositories.Implementations
{
    public class AttendanceRepository(EmployeeManagementSystemContext context):IAttendanceRepository
    {
        private readonly EmployeeManagementSystemContext _context=context;
        public async Task<bool> IsAttendanceFilled(DateOnly date,int employeeId)
        {
            return await _context.Attendances.AnyAsync(a => a.EmployeeId == employeeId && a.Date == date);
        }

        public async Task<List<Attendance>> GetAttendanceListByMonthYearEmployeeId(int employeeId,int year,int month)
        {
            return await _context.Attendances.Where(a=>a.Date.Month==month && a.Date.Year==year && a.EmployeeId==employeeId).ToListAsync();
        }

        public async Task<List<Attendance>> GetRecordsWithNullStatusForDate(DateOnly date)
        {
            return await _context.Attendances.Where(a =>a.Date==date && a.Status==null).ToListAsync();
        }

        public async Task<Attendance?> GetAttendanceByEmployeeAndDate(DateOnly date, int employeeId)
        {
            return await _context.Attendances.Where(a=>a.Date==date && a.EmployeeId==employeeId).FirstOrDefaultAsync();
        }
    }
}