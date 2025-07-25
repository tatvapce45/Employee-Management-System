using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.BusinessLogic.Dtos
{
    public class EmployeesResponseDto
    {
        public List<EmployeeDto> Employees{get;set;}=[];

        public int DepartmentId{get;set;}

        public int PageSize{get;set;}

        public int PageNumber{get;set;}

        public int TotalPages{get;set;}

        public int TotalEmployees{get;set;}

        public int CurrentPage{get;set;}

        public string SearchTerm{get;set;}=string.Empty;

        public string SortBy{get;set;}=string.Empty;

        public string SortOrder{get;set;}=string.Empty;

        public int LastEmployee{get;set;}
    }
}