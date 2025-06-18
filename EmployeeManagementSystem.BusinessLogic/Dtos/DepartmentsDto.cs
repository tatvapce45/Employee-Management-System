using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.BusinessLogic.Dtos
{
    public class DepartmrentDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class DepartmentsDto
    {
        public List<Department> Departments { get; set; } = [];
    }

    public class CreateDepartmentDto
    {
        public required string Name { get; set; }

        public string? Description { get; set; }
    }

    public class UpdateDepartmentDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }
    }
}