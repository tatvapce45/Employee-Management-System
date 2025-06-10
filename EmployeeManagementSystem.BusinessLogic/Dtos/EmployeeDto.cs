using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.BusinessLogic.Dtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime HiringDate { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; }

        public int DepartmentId { get; set; }

        public string Email { get; set; } = null!;

        public string MobileNo { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public int Age { get; set; }
    }

    public class CreateEmployeeDto
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string MobileNo { get; set; }

        [Required]
        public required string Gender { get; set; }

        [Required]
        public int Age { get; set; }
    }

    public class UpdateEmployeeDto
    {
        public int Id { get; set; }

        [Required]
        public required string Name { get; set; }

        public bool IsDeleted { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public required string Email { get; set; }

        [Required]
        public required string MobileNo { get; set; }

        [Required]
        public required string Gender { get; set; }

        [Required]
        public int Age { get; set; }
    }
}