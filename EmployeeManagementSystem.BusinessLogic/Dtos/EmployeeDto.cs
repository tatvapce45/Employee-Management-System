using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagementSystem.BusinessLogic.Dtos
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime HiringDate { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int DepartmentId { get; set; }

        public string Email { get; set; } = null!;

        public string MobileNo { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public int Age { get; set; }

        public decimal Salary { get; set; }

        public string DepartmentName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Zipcode { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        public int RoleId { get; set; }

        public string? UserName { get; set; }

        public IFormFile? ImageFile { get; set; }

        public byte[]? Image { get; set; }

        public string? ImageMimeType { get; set; }

        public string? ImageBase64 =>
            Image != null && !string.IsNullOrEmpty(ImageMimeType)
                ? $"data:{ImageMimeType};base64,{Convert.ToBase64String(Image)}"
                : null;

        public string? ImageUrl { get; set; }  

        public string? CloudinaryPublicId { get; set; }  
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

        [Required]
        public decimal Salary { get; set; }

        public string Address { get; set; } = string.Empty;

        public string Zipcode { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        public int RoleId { get; set; }

        public string? EmailBody { get; set; }

        public string? UserName { get; set; }

        public IFormFile? ImageFile { get; set; }
    }

    public class UpdateEmployeeDto
    {
        public int Id { get; set; }

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

        [Required]
        public decimal Salary { get; set; }

        public string Address { get; set; } = string.Empty;

        public string Zipcode { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        public int RoleId { get; set; }

        public string? UserName { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}