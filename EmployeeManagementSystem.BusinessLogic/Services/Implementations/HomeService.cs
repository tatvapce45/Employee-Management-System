using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Results;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.BusinessLogic.Services.Implementations
{
    public class HomeService(IEmployeesRepository employeesRepository) : IHomeService
    {
        private readonly IEmployeesRepository _employeesRepository = employeesRepository;

        public async Task<ServiceResult<DashboardResponseDto>> GetDashboardData(int timeId, string fromDate, string toDate)
        {
            DateTime? from = null;
            DateTime? to = null;
            if (DateTime.TryParse(fromDate, out var parsedFrom))
            {
                from = parsedFrom;
            }
            if (DateTime.TryParse(toDate, out var parsedTo))
            {
                to = parsedTo;
            }
            DateTime now = DateTime.Now;
            DateTime rangeStart = now;
            Dictionary<string, int> timeWiseEmployees = [];
            Dictionary<string, int> departmentWiseEmployees = [];
            Dictionary<string, int> genderWiseEmployees = [];
            Dictionary<string, int> countryWiseEmployees = [];
            Dictionary<int, int> ageWiseEmployees = [];
            if (timeId == 1)
            {
                var allEmployeesQuery = _employeesRepository.GetAllEmployees();

                var groupedData = await allEmployeesQuery
                    .GroupBy(e => new { e.HiringDate!.Year, e.HiringDate.Month })
                    .Select(g => new
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Total = g.Count()
                    })
                    .OrderBy(g => g.Year)
                    .ThenBy(g => g.Month)
                    .ToListAsync();

                timeWiseEmployees = groupedData
                    .ToDictionary(
                        x => $"{x.Year}-{x.Month:D2}",
                        x => x.Total
                    );
            }
            else if (timeId == 2)
            {
                rangeStart = now.Date.AddDays(-7);
                var last7DaysQuery = _employeesRepository.GetEmployeesDataForTime(rangeStart, now.Date);
                var dayNames = Enumerable.Range(0, 7)
                    .Select(i => rangeStart.AddDays(i).DayOfWeek.ToString())
                    .ToList();
                timeWiseEmployees = dayNames
                    .ToDictionary(day => day, _ => 0);
                var employeesGroup = await last7DaysQuery
                    .GroupBy(o => o.HiringDate!.DayOfWeek)
                    .Select(g => new
                    {
                        DayName = g.Key.ToString(),
                        Total = g.Count()
                    })
                    .ToListAsync();
                foreach (var eg in employeesGroup)
                {
                    if (timeWiseEmployees.ContainsKey(eg.DayName))
                    {
                        timeWiseEmployees[eg.DayName] = eg.Total;
                    }
                }
            }
            else if (timeId == 3)
            {
                rangeStart = now.Date.AddDays(-30);
                var last30Days = Enumerable.Range(0, 30)
                                .Select(i => now.Date.AddDays(-i))
                                .OrderBy(d => d)
                                .ToList();
                timeWiseEmployees = last30Days
                .ToDictionary(date => date.ToString("yyyy-MM-dd"), _ => 0);


                var last30DaysQuery = _employeesRepository.GetEmployeesDataForTime(rangeStart, now.Date);

                var groupedData = await last30DaysQuery
                                .GroupBy(e => e.HiringDate!.Date)
                                .Select(g => new { Date = g.Key, Total = g.Count() })
                                .ToListAsync();
                foreach (var item in groupedData)
                {
                    string key = item.Date.ToString("yyyy-MM-dd");
                    if (timeWiseEmployees.ContainsKey(key))
                    {
                        timeWiseEmployees[key] = item.Total;
                    }
                }
            }
            else if (timeId == 4)
            {
                var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
                var today = now.Date;

                var datesInMonth = Enumerable.Range(0, (today - firstDayOfMonth).Days + 1)
                    .Select(i => firstDayOfMonth.AddDays(i))
                    .ToList();

                timeWiseEmployees = datesInMonth.ToDictionary(date => date.ToString("yyyy-MM-dd"), _ => 0);

                var employeesInMonthQuery = _employeesRepository.GetEmployeesDataForTime(firstDayOfMonth, today);

                var employeesGroup = await employeesInMonthQuery
                    .GroupBy(e => e.HiringDate!.Date)
                    .Select(g => new { Date = g.Key, Total = g.Count() })
                    .ToListAsync();

                foreach (var eg in employeesGroup)
                {
                    var key = eg.Date.ToString("yyyy-MM-dd");
                    if (timeWiseEmployees.ContainsKey(key))
                    {
                        timeWiseEmployees[key] = eg.Total;
                    }
                }
            }
            else if (timeId == 5 && from.HasValue && to.HasValue)
            {
                var diffDays = (to.Value.Date - from.Value.Date).Days + 1;

                if (diffDays <= 7)
                {
                    var dayNames = Enumerable.Range(0, diffDays)
                        .Select(i => from.Value.Date.AddDays(i).DayOfWeek.ToString())
                        .ToList();

                    timeWiseEmployees = dayNames.ToDictionary(day => day, _ => 0);

                    var employeesGroup = await _employeesRepository.GetEmployeesDataForTime(from.Value, to.Value)
                        .GroupBy(e => e.HiringDate!.DayOfWeek)
                        .Select(g => new
                        {
                            DayName = g.Key.ToString(),
                            Total = g.Count()
                        })
                        .ToListAsync();

                    foreach (var eg in employeesGroup)
                    {
                        if (timeWiseEmployees.ContainsKey(eg.DayName))
                        {
                            timeWiseEmployees[eg.DayName] = eg.Total;
                        }
                    }
                }
                else if (diffDays > 7 && diffDays <= 31)
                {
                    var dates = Enumerable.Range(0, diffDays)
                        .Select(i => from.Value.Date.AddDays(i))
                        .ToList();

                    timeWiseEmployees = dates.ToDictionary(date => date.ToString("yyyy-MM-dd"), _ => 0);

                    var employeesGroup = await _employeesRepository.GetEmployeesDataForTime(from.Value, to.Value)
                        .GroupBy(e => e.HiringDate!.Date)
                        .Select(g => new
                        {
                            Date = g.Key,
                            Total = g.Count()
                        })
                        .ToListAsync();

                    foreach (var eg in employeesGroup)
                    {
                        var key = eg.Date.ToString("yyyy-MM-dd");
                        if (timeWiseEmployees.ContainsKey(key))
                        {
                            timeWiseEmployees[key] = eg.Total;
                        }
                    }
                }
                else
                {
                    var months = new List<string>();
                    var current = new DateTime(from.Value.Year, from.Value.Month, 1);
                    var end = new DateTime(to.Value.Year, to.Value.Month, 1);

                    while (current <= end)
                    {
                        months.Add(current.ToString("yyyy-MM"));
                        current = current.AddMonths(1);
                    }

                    timeWiseEmployees = months.ToDictionary(m => m, _ => 0);

                    var employeesGroup = await _employeesRepository.GetEmployeesDataForTime(from.Value, to.Value)
                        .GroupBy(e => new { e.HiringDate!.Year, e.HiringDate.Month })
                        .Select(g => new
                        {
                            Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("yyyy-MM"),
                            Total = g.Count()
                        })
                        .ToListAsync();

                    foreach (var eg in employeesGroup)
                    {
                        if (timeWiseEmployees.ContainsKey(eg.Month))
                        {
                            timeWiseEmployees[eg.Month] = eg.Total;
                        }
                    }
                }
            }

            var departmentWiseEmployeesFromDb = await _employeesRepository.GetAllEmployees()
               .Where(e => !string.IsNullOrEmpty(e.Department!.Name))
               .GroupBy(e => e.Department!.Name)
               .Select(g => new
               {
                   Department = g.Key!,
                   Count = g.Count()
               })
               .ToListAsync();

            foreach (var eg in departmentWiseEmployeesFromDb)
            {
                departmentWiseEmployees[eg.Department] = eg.Count;
            }

            var genderWiseEmployeesFromDb = await _employeesRepository.GetAllEmployees()
               .Where(e => !string.IsNullOrEmpty(e.Gender))
               .GroupBy(e => e.Gender)
               .Select(g => new
               {
                   Gender = g.Key!,
                   Count = g.Count()
               })
               .ToListAsync();

            foreach (var eg in genderWiseEmployeesFromDb)
            {
                genderWiseEmployees[eg.Gender] = eg.Count;
            }

            var ageWiseEmployeesFromDb = await _employeesRepository.GetAllEmployees()
               .GroupBy(e => e.Age)
               .Select(g => new
               {
                   Age = g.Key!,
                   Count = g.Count()
               })
               .ToListAsync();

            foreach (var eg in ageWiseEmployeesFromDb)
            {
                ageWiseEmployees[eg.Age] = eg.Count;
            }

            var countryWiseEmployeesFromDb = await _employeesRepository.GetAllEmployees()
                .Where(e => !string.IsNullOrEmpty(e.Country!.Name))
               .GroupBy(e => e.Country.Name)
               .Select(g => new
               {
                   Country = g.Key!,
                   Count = g.Count()
               })
               .ToListAsync();

            foreach (var eg in countryWiseEmployeesFromDb)
            {
                countryWiseEmployees[eg.Country] = eg.Count;
            }

            DashboardResponseDto dashboardResponseDto = new()
            {
                TimeWiseEmployees = timeWiseEmployees,
                DepartmentWiseEmployees = departmentWiseEmployees,
                GenderWiseEmployees = genderWiseEmployees,
                AgeWiseEmployees = ageWiseEmployees,
                CountryWiseEmployees = countryWiseEmployees
            };
            return ServiceResult<DashboardResponseDto>.Ok(dashboardResponseDto, "Here is the data.");
        }
    }
}