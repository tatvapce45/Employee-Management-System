using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Results;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;

namespace EmployeeManagementSystem.BusinessLogic.Services.Implementations
{
    public class EmployeesService(IEmployeesRepository employeesRepository, IGenericRepository<Employee> employeeGenericRepository) : IEmployeesService
    {
        private readonly IEmployeesRepository _employeesRepository = employeesRepository;
        private readonly IGenericRepository<Employee> _employeeGenericRepository = employeeGenericRepository;
        public async Task<ServiceResult<EmployeesResponseDto>> GetEmployees(EmployeesRequestDto employeesRequestDto)
        {
            try
            {
                var employees = await _employeesRepository.GetEmployeesAsync(employeesRequestDto.DepartmentId, employeesRequestDto.PageNumber, employeesRequestDto.PageSize, employeesRequestDto.SortBy, employeesRequestDto.SortOrder, employeesRequestDto.SearchTerm);
                if (employees == null || employees.Count == 0)
                {
                    return ServiceResult<EmployeesResponseDto>.NotFound("No employees found for the given criteria.");
                }
                var employeesResponseDto = new EmployeesResponseDto()
                {
                    Employees = employees,
                    TotalEmployees = employees.Count,
                    CurrentPage = employeesRequestDto.PageNumber,
                    PageSize = employeesRequestDto.PageSize,
                    TotalPages = (int)Math.Ceiling((double)employees.Count / employeesRequestDto.PageSize),
                    SortBy = employeesRequestDto.SortBy,
                    SortOrder = employeesRequestDto.SortOrder,
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
                        Employee employee = result.Data!;
                        EmployeeDto employeeDto = new()
                        {
                            Id = employee.Id,
                            Name = employee.Name,
                            DepartmentId = employee.DepartmentId,
                            Email = employee.Email,
                            MobileNo = employee.MobileNo,
                            Gender = employee.Gender,
                            Age = employee.Age,
                            HiringDate = employee.HiringDate,
                            UpdatedAt = employee.UpdatedAt
                        };
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
                bool ifAlreadyExists = await _employeesRepository.CheckIfExists(createEmployeeDto.Email, createEmployeeDto.MobileNo);
                if (ifAlreadyExists)
                {
                    return ServiceResult<EmployeeDto>.BadRequest("Employee with same email or mobile number already exists!");
                }
                else
                {
                    Employee employee = new()
                    {
                        Email = createEmployeeDto.Email,
                        Name = createEmployeeDto.Name,
                        Gender = createEmployeeDto.Gender,
                        Age = createEmployeeDto.Age,
                        DepartmentId = createEmployeeDto.DepartmentId,
                        MobileNo = createEmployeeDto.MobileNo,
                        HiringDate = DateTime.Now
                    };
                    var result = await _employeeGenericRepository.AddAsync(employee);
                    if (!result.Success)
                    {
                        return ServiceResult<EmployeeDto>.InternalError($"Failed to register user: {result.ErrorMessage}");
                    }

                    EmployeeDto employeeDto = new()
                    {
                        Id = employee.Id,
                        Name = createEmployeeDto.Name,
                        Email = createEmployeeDto.Email,
                        MobileNo = createEmployeeDto.MobileNo,
                        Gender = createEmployeeDto.Gender,
                        Age = createEmployeeDto.Age,
                        DepartmentId = createEmployeeDto.DepartmentId,
                        HiringDate = DateTime.Now
                    };
                    return ServiceResult<EmployeeDto>.Created(employeeDto, "User registered successfully.");
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
                bool ifAlreadyExists = await _employeesRepository.CheckIfExistsWithDifferentId(updateEmployeeDto.Id, updateEmployeeDto.Email, updateEmployeeDto.MobileNo);
                if (ifAlreadyExists)
                {
                    return ServiceResult<EmployeeDto>.BadRequest("Employee with same email or mobile number already exists!");
                }
                else
                {
                    var result = await _employeeGenericRepository.GetById(updateEmployeeDto.Id);
                    if (result.Success)
                    {
                        if (result.Data != null)
                        {
                            Employee employee = result.Data;
                            employee.Email = updateEmployeeDto.Email;
                            employee.MobileNo = updateEmployeeDto.MobileNo;
                            employee.Gender = updateEmployeeDto.Gender;
                            employee.Age = updateEmployeeDto.Age;
                            employee.DepartmentId = updateEmployeeDto.DepartmentId;
                            employee.UpdatedAt = DateTime.Now;
                            employee.Name = updateEmployeeDto.Name;
                            var updateResult = await _employeeGenericRepository.UpdateAsync(employee);
                            if (updateResult.Success)
                            {
                                EmployeeDto employeeDto = new()
                                {
                                    Id = employee.Id,
                                    Name = employee.Name,
                                    Email = employee.Email,
                                    MobileNo = employee.MobileNo,
                                    Gender = employee.Gender,
                                    Age = employee.Age,
                                    DepartmentId = employee.DepartmentId,
                                    HiringDate = employee.HiringDate
                                };
                                return ServiceResult<EmployeeDto>.Ok(employeeDto, "Employee updated successfully.");
                            }
                            else
                            {
                                return ServiceResult<EmployeeDto>.InternalError("Error updating employee", new Exception(updateResult.ErrorMessage));
                            }
                        }
                        else
                        {
                            return ServiceResult<EmployeeDto>.BadRequest("Employee not found!");
                        }
                    }
                    else
                    {
                        return ServiceResult<EmployeeDto>.InternalError("An unexpected error occurred while fetching employee.", new Exception(result.ErrorMessage));
                    }
                }
            }
            catch (Exception ex)
            {
                return ServiceResult<EmployeeDto>.InternalError("An unexpected error occurred while fetching employee.", ex);
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
    }
}