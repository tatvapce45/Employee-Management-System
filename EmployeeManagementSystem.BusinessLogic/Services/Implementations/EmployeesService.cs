using AutoMapper;
using OfficeOpenXml;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Results;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using EmployeeManagementSystem.BusinessLogic.Helpers;

namespace EmployeeManagementSystem.BusinessLogic.Services.Implementations
{
    public class EmployeesService(IEmployeesRepository employeesRepository, IDepartmentsRepository departmentsRepository, IGenericRepository<Employee> employeeGenericRepository, IGenericRepository<Department> departmentGenericRepository,HashHelper hashHelper, IMapper mapper) : IEmployeesService
    {
        private readonly IEmployeesRepository _employeesRepository = employeesRepository;
        private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
        private readonly IGenericRepository<Employee> _employeeGenericRepository = employeeGenericRepository;
        private readonly IGenericRepository<Department> _departmentGenericRepository = departmentGenericRepository;
        private readonly HashHelper _hashHelper = hashHelper;
        private readonly IMapper _mapper = mapper;
        public async Task<ServiceResult<EmployeesResponseDto>> GetEmployees(EmployeesRequestDto employeesRequestDto)
        {
            try
            {
                var employeesData = await _employeesRepository.GetEmployeesAsync(employeesRequestDto.DepartmentId, employeesRequestDto.PageNumber, employeesRequestDto.PageSize, employeesRequestDto.SortBy, employeesRequestDto.SortOrder, employeesRequestDto.SearchTerm);
                if (employeesData.Items == null || employeesData.TotalCount == 0)
                {
                    return ServiceResult<EmployeesResponseDto>.NotFound("No employees found for the given criteria.");
                }
                List<EmployeeDto> employeeDtos = _mapper.Map<List<EmployeeDto>>(employeesData.Items);
                var employeesResponseDto = new EmployeesResponseDto()
                {
                    Employees = employeeDtos,
                    TotalEmployees = employeesData.TotalCount,
                    CurrentPage = employeesRequestDto.PageNumber,
                    PageSize = employeesRequestDto.PageSize,
                    TotalPages = (int)Math.Ceiling((double)employeesData.TotalCount / employeesRequestDto.PageSize),
                    SortBy = employeesRequestDto.SortBy,
                    SortOrder = employeesRequestDto.SortOrder,
                    DepartmentId = employeesRequestDto.DepartmentId,
                    LastEmployee = Math.Min(employeesRequestDto.PageNumber * employeesRequestDto.PageSize, employeesData.TotalCount)
                };
                return ServiceResult<EmployeesResponseDto>.Ok(employeesResponseDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeesResponseDto>.InternalError("An unexpected error occurred while fetching employees.", ex);
            }
        }

        public async Task<ServiceResult<EmployeeDto>> GetEmployee(int employeeId)
        {
            try
            {
                var result = await _employeeGenericRepository.GetById(employeeId);
                if (result.Success)
                {
                    if (result.Data != null)
                    {
                        var employeeDto = _mapper.Map<EmployeeDto>(result.Data);
                        return ServiceResult<EmployeeDto>.Ok(employeeDto);
                    }
                    else
                    {
                        return ServiceResult<EmployeeDto>.NotFound("Employee with provided id not found!");
                    }
                }
                else
                {
                    return ServiceResult<EmployeeDto>.InternalError("An unexpected error occurred while fetching employee.", new Exception(result.ErrorMessage));
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDto>.InternalError("An unexpected error occurred while fetching employee.", ex);
            }
        }

        public async Task<ServiceResult<EmployeeDto>> AddEmployee(CreateEmployeeDto createEmployeeDto)
        {
            try
            {
                bool ifDepartmentExists = await _departmentsRepository.CheckIfExists(createEmployeeDto.DepartmentId);
                if (!ifDepartmentExists)
                {
                    return ServiceResult<EmployeeDto>.BadRequest("Department with provided id does not exist!");
                }
                else
                {
                    bool ifAlreadyExists = await _employeesRepository.CheckIfExists(createEmployeeDto.Email, createEmployeeDto.MobileNo);
                    if (ifAlreadyExists)
                    {
                        return ServiceResult<EmployeeDto>.BadRequest("Employee with same email or mobile number already exists!");
                    }

                    var employee = _mapper.Map<Employee>(createEmployeeDto);

                    var result = await _employeeGenericRepository.AddAsync(employee);
                    if (!result.Success)
                    {
                        return ServiceResult<EmployeeDto>.InternalError($"Failed to register user: {result.ErrorMessage}");
                    }

                    var employeeDto = _mapper.Map<EmployeeDto>(employee);
                    return ServiceResult<EmployeeDto>.Created(null, "User registered successfully.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDto>.InternalError("An unexpected error occurred during adding employee.", ex);
            }
        }

        public async Task<ServiceResult<EmployeeDto>> UpdateEmployee(UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                bool ifDepartmentExists = await _departmentsRepository.CheckIfExists(updateEmployeeDto.DepartmentId);
                if (!ifDepartmentExists)
                {
                    return ServiceResult<EmployeeDto>.BadRequest("Department with provided id does not exist!");
                }
                else
                {
                    bool ifAlreadyExists = await _employeesRepository.CheckIfExistsWithDifferentId(updateEmployeeDto.Id, updateEmployeeDto.Email, updateEmployeeDto.MobileNo);
                    if (ifAlreadyExists)
                    {
                        return ServiceResult<EmployeeDto>.BadRequest("Employee with same email or mobile number already exists!");
                    }

                    var result = await _employeeGenericRepository.GetById(updateEmployeeDto.Id);
                    if (!result.Success)
                    {
                        return ServiceResult<EmployeeDto>.InternalError("An unexpected error occurred while fetching employee.", new Exception(result.ErrorMessage));
                    }

                    if (result.Data == null)
                    {
                        return ServiceResult<EmployeeDto>.BadRequest("Employee not found!");
                    }

                    Employee employee = result.Data;
                    _mapper.Map(updateEmployeeDto, employee);
                    employee.UpdatedAt = DateTime.Now;

                    var updateResult = await _employeeGenericRepository.UpdateAsync(employee);
                    if (!updateResult.Success)
                    {
                        return ServiceResult<EmployeeDto>.InternalError("Error updating employee", new Exception(updateResult.ErrorMessage));
                    }

                    EmployeeDto employeeDto = _mapper.Map<EmployeeDto>(updateResult.Data);
                    return ServiceResult<EmployeeDto>.Ok(null, "Employee updated successfully.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDto>.InternalError("An unexpected error occurred while updating employee.", ex);
            }
        }

        public async Task<ServiceResult<EmployeeDto>> DeleteEmployee(int employeeId)
        {
            try
            {
                var result = await _employeeGenericRepository.GetById(employeeId);
                if (result.Data == null)
                {
                    return ServiceResult<EmployeeDto>.NotFound("Employee with provided id not found!");
                }
                else
                {
                    var deleteResult = await _employeeGenericRepository.DeleteAsync(result.Data);
                    if (deleteResult.Success)
                    {
                        return ServiceResult<EmployeeDto>.Ok(null, "Employee deleted successfully.");
                    }
                    else
                    {
                        return ServiceResult<EmployeeDto>.InternalError("An unexpected error occurred while deleting employee.", new Exception(deleteResult.ErrorMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDto>.InternalError("An unexpected error occurred while deleting employee.", ex);
            }
        }

        public async Task<ServiceResult<string>> DeleteMultipleEmployees(int[] employeeIds)
        {
            try
            {
                if (employeeIds == null || employeeIds.Length == 0)
                {
                    return ServiceResult<string>.BadRequest("No employee IDs provided.");
                }

                var failedDeletions = new List<int>();

                foreach (var id in employeeIds)
                {
                    var result = await _employeeGenericRepository.GetById(id);
                    if (result.Data == null)
                    {
                        failedDeletions.Add(id);
                        continue;
                    }

                    var deleteResult = await _employeeGenericRepository.DeleteAsync(result.Data);
                    if (!deleteResult.Success)
                    {
                        failedDeletions.Add(id);
                    }
                }

                if (failedDeletions.Count == 0)
                {
                    return ServiceResult<string>.Ok(null, "All selected employees deleted successfully.");
                }
                else
                {
                    string message = $"Some employees could not be deleted. Failed IDs: {string.Join(", ", failedDeletions)}";
                    return ServiceResult<string>.PartialSuccess(message);
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.InternalError("An unexpected error occurred while deleting employees.", ex);
            }
        }

        public async Task<ServiceResult<object>> GenerateEmployeesReport(int departnemtId, string? fromDate, string? toDate, string? gender, int? age)
        {
            try
            {
                DateOnly? from = string.IsNullOrWhiteSpace(fromDate) ? null : DateOnly.Parse(fromDate);
                DateOnly? to = string.IsNullOrWhiteSpace(toDate) ? null : DateOnly.Parse(toDate);
                var result = await _employeesRepository.GetEmployeesForReport(departnemtId, from, to, gender, age);
                int totalEmployees = 0;

                if (result == null || result.Count == 0)
                {
                    return ServiceResult<object>.NotFound("No employees found for the given criteria.");
                }
                else
                {
                    totalEmployees = result.Count;
                    decimal averageSalary = result.Select(h => h.Salary).Average();
                    var report = new
                    {
                        Employees = result,
                        TotalEmployees = totalEmployees,
                        AverageSalary = averageSalary
                    };
                    return ServiceResult<object>.Ok(report, "Employees report generated successfully.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<object>.InternalError("An unexpected error occurred while generating report.", ex);
            }
        }

        public async Task<ServiceResult<byte[]>> GenerateEmployeesReportExcel(int departmentId, string? fromDate, string? toDate, string? gender, int? age)
        {
            try
            {
                var reportResult = await GenerateEmployeesReport(departmentId, fromDate, toDate, gender, age);

                if (!reportResult.Success || reportResult.Data == null)
                {
                    return ServiceResult<byte[]>.NotFound("No employees found for the given criteria.");
                }

                dynamic report = reportResult.Data;
                var employees = ((IEnumerable<dynamic>)report.Employees).ToList();


                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Employees Report");

                worksheet.Cells[1, 1].Value = "Employee Id";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Gender";
                worksheet.Cells[1, 4].Value = "Age";
                worksheet.Cells[1, 5].Value = "Salary";
                int row = 2;
                foreach (var emp in employees)
                {
                    worksheet.Cells[row, 1].Value = emp.Id;
                    worksheet.Cells[row, 2].Value = emp.Name;
                    worksheet.Cells[row, 3].Value = emp.Gender;
                    worksheet.Cells[row, 4].Value = emp.Age;
                    worksheet.Cells[row, 5].Value = emp.Salary;
                    row++;
                }

                worksheet.Cells[row + 1, 1].Value = "Total Employees:";
                worksheet.Cells[row + 1, 2].Value = report.TotalEmployees;

                worksheet.Cells[row + 2, 1].Value = "Average Salary:";
                worksheet.Cells[row + 2, 2].Value = Math.Round(report.AverageSalary, 2);

                using (var range = worksheet.Cells[1, 1, 1, 5])
                {
                    range.Style.Font.Bold = true;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                var excelBytes = package.GetAsByteArray();

                return ServiceResult<byte[]>.Ok(excelBytes, "Excel report generated successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<byte[]>.InternalError("Failed to generate Excel report.", ex);
            }
        }

        public async Task<ServiceResult<DepartmentsDto>> GetDepartments()
        {
            try
            {
                var departments = await _departmentsRepository.GetAllDepartments();
                if (departments == null || departments.Count == 0)
                {
                    return ServiceResult<DepartmentsDto>.NotFound("No departments found.");
                }
                var departmentsDto = new DepartmentsDto()
                {
                    Departments = departments
                };
                return ServiceResult<DepartmentsDto>.Ok(departmentsDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<DepartmentsDto>.InternalError("An unexpected error occurred while fetching departments.", ex);
            }
        }

        public async Task<ServiceResult<DepartmrentDto>> GetDepartment(int departmentId)
        {
            try
            {
                var result = await _departmentGenericRepository.GetById(departmentId);
                if (result.Success)
                {
                    if (result.Data != null)
                    {
                        var deaprtmentDto = _mapper.Map<DepartmrentDto>(result.Data);
                        return ServiceResult<DepartmrentDto>.Ok(deaprtmentDto);
                    }
                    else
                    {
                        return ServiceResult<DepartmrentDto>.NotFound("Department with provided id not found!");
                    }
                }
                else
                {
                    return ServiceResult<DepartmrentDto>.InternalError("An unexpected error occurred while fetching department.", new Exception(result.ErrorMessage));
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<DepartmrentDto>.InternalError("An unexpected error occurred while fetching department.", ex);
            }
        }

        public async Task<ServiceResult<DepartmrentDto>> AddDepartment(CreateDepartmentDto createDepartmentDto)
        {
            try
            {
                bool ifDepartmentExists = await _departmentsRepository.CheckIfExistsWithName(createDepartmentDto.Name);
                if (ifDepartmentExists)
                {
                    return ServiceResult<DepartmrentDto>.BadRequest("Department with provided name already exists!");
                }
                else
                {
                    var department = _mapper.Map<Department>(createDepartmentDto);

                    var result = await _departmentGenericRepository.AddAsync(department);
                    if (!result.Success)
                    {
                        return ServiceResult<DepartmrentDto>.InternalError($"Failed to add department: {result.ErrorMessage}");
                    }

                    var departmentDto = _mapper.Map<DepartmrentDto>(department);
                    return ServiceResult<DepartmrentDto>.Created(null, "Department added successfully.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<DepartmrentDto>.InternalError("An unexpected error occurred during adding department.", ex);
            }
        }

        public async Task<ServiceResult<DepartmrentDto>> UpdateDepartment(UpdateDepartmentDto updateDepartmentDto)
        {
            try
            {
                bool ifDepartmentExists = await _departmentsRepository.CheckIfExistsWithNameAndDiffId(updateDepartmentDto.Name, updateDepartmentDto.Id);
                if (ifDepartmentExists)
                {
                    return ServiceResult<DepartmrentDto>.BadRequest("Department with provided name exists with different id!");
                }
                else
                {
                    var result = await _departmentGenericRepository.GetById(updateDepartmentDto.Id);
                    if (!result.Success)
                    {
                        return ServiceResult<DepartmrentDto>.InternalError("An unexpected error occurred while fetching department.", new Exception(result.ErrorMessage));
                    }

                    if (result.Data == null)
                    {
                        return ServiceResult<DepartmrentDto>.BadRequest("Department not found!");
                    }

                    Department department = result.Data;
                    _mapper.Map(updateDepartmentDto, department);
                    department.UpdatedAt = DateTime.Now;

                    var updateResult = await _departmentGenericRepository.UpdateAsync(department);
                    if (!updateResult.Success)
                    {
                        return ServiceResult<DepartmrentDto>.InternalError("Error updating department", new Exception(updateResult.ErrorMessage));
                    }

                    DepartmrentDto departmrentDto = _mapper.Map<DepartmrentDto>(updateResult.Data);
                    return ServiceResult<DepartmrentDto>.Ok(null, "Department updated successfully.");
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<DepartmrentDto>.InternalError("An unexpected error occurred while updating department.", ex);
            }
        }

        public async Task<ServiceResult<DepartmrentDto>> DeleteDepartment(int departmentId)
        {
            try
            {
                var result = await _departmentGenericRepository.GetById(departmentId);
                if (result.Data == null)
                {
                    return ServiceResult<DepartmrentDto>.NotFound("Department with provided id not found!");
                }
                else
                {
                    var deleteResult = await _departmentGenericRepository.DeleteAsync(result.Data);
                    if (deleteResult.Success)
                    {
                        return ServiceResult<DepartmrentDto>.Ok(null, "Department deleted successfully.");
                    }
                    else
                    {
                        return ServiceResult<DepartmrentDto>.InternalError("An unexpected error occurred while deleting department.", new Exception(deleteResult.ErrorMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<DepartmrentDto>.InternalError("An unexpected error occurred while deleting department.", ex);
            }
        }

        public async Task<ServiceResult<UpdateProfileDto>> GetEmployeeForMyProfile(int employeeId)
        {
            try
            {
                var result = await _employeeGenericRepository.GetById(employeeId);
                if (result.Success)
                {
                    if (result.Data != null)
                    {
                        var employeeDto = _mapper.Map<UpdateProfileDto>(result.Data);
                        return ServiceResult<UpdateProfileDto>.Ok(employeeDto);
                    }
                    else
                    {
                        return ServiceResult<UpdateProfileDto>.NotFound("Employee with provided id not found!");
                    }
                }
                else
                {
                    return ServiceResult<UpdateProfileDto>.InternalError("An unexpected error occurred while fetching employee.", new Exception(result.ErrorMessage));
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<UpdateProfileDto>.InternalError("An unexpected error occurred while fetching employee.", ex);
            }
        }

        public async Task<ServiceResult<EmployeeDto>> UpdateProfile(UpdateProfileDto updateProfileDto)
        {
            try
            {

                bool ifAlreadyExists = await _employeesRepository.CheckIfExistsWithDifferentId(updateProfileDto.Id, updateProfileDto.Email, updateProfileDto.MobileNo);
                if (ifAlreadyExists)
                {
                    return ServiceResult<EmployeeDto>.BadRequest("User with same email or mobile number already exists!");
                }

                var result = await _employeeGenericRepository.GetById(updateProfileDto.Id);
                if (!result.Success)
                {
                    return ServiceResult<EmployeeDto>.InternalError("An unexpected error occurred while fetching User.", new Exception(result.ErrorMessage));
                }

                if (result.Data == null)
                {
                    return ServiceResult<EmployeeDto>.BadRequest("User not found!");
                }

                Employee employee = result.Data;
                _mapper.Map(updateProfileDto, employee);
                employee.UpdatedAt = DateTime.Now;

                var updateResult = await _employeeGenericRepository.UpdateAsync(employee);
                if (!updateResult.Success)
                {
                    return ServiceResult<EmployeeDto>.InternalError("Error updating User", new Exception(updateResult.ErrorMessage));
                }

                EmployeeDto employeeDto = _mapper.Map<EmployeeDto>(updateResult.Data);
                return ServiceResult<EmployeeDto>.Ok(null, "User updated successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDto>.InternalError("An unexpected error occurred while updating User.", ex);
            }
        }

        public async Task<ServiceResult<ChangePasswordDto>> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var result = await _employeeGenericRepository.GetById(changePasswordDto.Id);
                if (!result.Success)
                {
                    return ServiceResult<ChangePasswordDto>.InternalError("An unexpected error occurred while fetching User.", new Exception(result.ErrorMessage));
                }

                if (result.Data == null)
                {
                    return ServiceResult<ChangePasswordDto>.BadRequest("User not found!");
                }

                Employee employee = result.Data;
                string correctPassword=_hashHelper.Decrypt(employee.Password);
                if (correctPassword != changePasswordDto.CurrentPassword)
                {
                    return ServiceResult<ChangePasswordDto>.BadRequest("Provided current password is incorrect");
                }
                else if(correctPassword==changePasswordDto.NewPassword)
                {
                    return ServiceResult<ChangePasswordDto>.BadRequest("Old and new password cannot be same");
                }
                string encryptedNewPassword=_hashHelper.Encrypt(changePasswordDto.NewPassword);
                employee.Password=encryptedNewPassword;
                employee.UpdatedAt = DateTime.Now;

                var updateResult = await _employeeGenericRepository.UpdateAsync(employee);
                if (!updateResult.Success)
                {
                    return ServiceResult<ChangePasswordDto>.InternalError("Error changing password", new Exception(updateResult.ErrorMessage));
                }
                return ServiceResult<ChangePasswordDto>.Ok(null, "Password updated successfully.");
            }
            catch (Exception ex)
            {
                return ServiceResult<ChangePasswordDto>.InternalError("An unexpected error occurred while updating Password.", ex);
            }
        }
    }
}