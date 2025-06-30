using AutoMapper;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Results;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using EmployeeManagementSystem.DataAccess.Results;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.BusinessLogic.Services.Implementations
{
    public class AttendanceService(IAttendanceRepository attendanceRepository, IGenericRepository<Attendance> attendanceGenericRepository, IEmployeesRepository employeesRepository, NotificationService notificationService, IMapper mapper) : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;
        private readonly IGenericRepository<Attendance> _attendanceGenericRepository = attendanceGenericRepository;
        private readonly IEmployeesRepository _employeesRepository = employeesRepository;
        private readonly NotificationService _notificationService = notificationService;
        private readonly IMapper _mapper = mapper;
        public async Task<ServiceResult<string>> MarkAttendance(MarkAttendanceDto markAttendanceDto)
        {
            try
            {
                var attendanceData = await _attendanceRepository.GetAttendanceByEmployeeAndDate(DateOnly.FromDateTime(DateTime.Now), markAttendanceDto.EmployeeId);

                if (attendanceData != null)
                {
                    attendanceData.Status = markAttendanceDto.Status;
                    var updateResult = await _attendanceGenericRepository.UpdateAsync(attendanceData);
                    if (!updateResult.Success)
                    {
                        return ServiceResult<string>.InternalError("An unexpected error occurred while marking attendance.", new Exception(updateResult.ErrorMessage));
                    }
                    return ServiceResult<string>.Ok(null, "Attendance marked successfully!");
                }
                Attendance attendance = _mapper.Map<Attendance>(markAttendanceDto);
                RepositoryResult<Attendance> result = await _attendanceGenericRepository.AddAsync(attendance);
                if (!result.Success)
                {
                    return ServiceResult<string>.InternalError($"Failed to mark attendance: {result.ErrorMessage}");
                }
                return ServiceResult<string>.Created(null, "Attendance marked successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.InternalError("An unexpected error occurred during marking attendance.", ex);
            }
        }

        public async Task<ServiceResult<string>> UpdateAttendance(UpdateAttendanceDto updateAttendanceDto)
        {
            try
            {
                var result = await _attendanceGenericRepository.GetById(updateAttendanceDto.Id);
                if (!result.Success)
                {
                    return ServiceResult<string>.InternalError("An unexpected error occurred while retrieving attendance.");
                }

                var existingAttendance = result.Data;
                if (existingAttendance == null)
                {
                    return ServiceResult<string>.NotFound("Attendance not found.");
                }

                _mapper.Map(updateAttendanceDto, existingAttendance);
                if (existingAttendance.CheckInTime.HasValue)
                {
                    existingAttendance.CheckInTime = DateTime.SpecifyKind(existingAttendance.CheckInTime.Value, DateTimeKind.Unspecified);
                }

                if (existingAttendance.CheckOutTime.HasValue)
                {
                    existingAttendance.CheckOutTime = DateTime.SpecifyKind(existingAttendance.CheckOutTime.Value, DateTimeKind.Unspecified);
                }

                var updateResult = await _attendanceGenericRepository.UpdateAsync(existingAttendance);
                if (!updateResult.Success)
                {
                    return ServiceResult<string>.InternalError("An unexpected error occurred while updating attendance.", new Exception(updateResult.ErrorMessage));
                }

                return ServiceResult<string>.Ok(null, "Attendance updated successfully!");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.InternalError("An unexpected error occurred during updating attendance.", ex);
            }
        }

        public async Task<ServiceResult<AttendanceListDto>> GetAttendanceList(AttendanceListRequestDto attendanceListRequestDto)
        {
            try
            {
                List<Attendance> attendances = await _attendanceRepository.GetAttendanceListByMonthYearEmployeeId(attendanceListRequestDto.EmployeeId, attendanceListRequestDto.Year, attendanceListRequestDto.Month);
                List<AttendanceDto> attendanceDtos = _mapper.Map<List<AttendanceDto>>(attendances);
                AttendanceListDto attendanceListDto = new()
                {
                    AttendanceDtos = attendanceDtos,
                    Year = attendanceListRequestDto.Year,
                    Month = attendanceListRequestDto.Month
                };
                return ServiceResult<AttendanceListDto>.Ok(attendanceListDto, "Here are the list of attendances");
            }
            catch (Exception ex)
            {
                return ServiceResult<AttendanceListDto>.InternalError("error fetching attendance list", ex);
            }
        }

        private DateTime GetIndiaTimeNow()
        {
            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone);
        }

        public async Task AddEmployeeStatusAtMidnight()
        {
            var indiaTime = GetIndiaTimeNow();

            if (indiaTime.Hour == 0 && indiaTime.Minute == 0)
            {
                await AddEmployeeStatusEntriesForToday();
            }
        }

        public async Task NotifyEmployeesAt6PM()
        {
            var indiaTime = GetIndiaTimeNow();

            if (indiaTime.Hour == 18 && indiaTime.Minute == 0)
            {
                await NotifyEmployeesWithNullStatus();
            }
        }

        public async Task AddEmployeeStatusEntriesForToday()
        {
            var allEmployeesQuery = _employeesRepository.GetAllEmployees();
            List<Employee> employees = await allEmployeesQuery.ToListAsync();
            foreach (var employee in employees)
            {
                await _attendanceGenericRepository.AddAsync(new Attendance
                {
                    EmployeeId = employee.Id,
                    Date = DateOnly.FromDateTime(DateTime.Now),
                    Status = null
                });
            }
        }

        public async Task NotifyEmployeesWithNullStatus()
        {
            var records = await _attendanceRepository.GetRecordsWithNullStatusForDate(DateOnly.FromDateTime(DateTime.Now));

            foreach (var record in records)
            {
                var employee = await _employeesRepository.GetEmployeeById(record.EmployeeId);
                if (employee != null && !string.IsNullOrEmpty(employee.Email))
                {
                    await _notificationService.SendNotificationToUserAsync(employee.Email, "Please update your attendance status for today.");
                }
            }
        }
    }
}