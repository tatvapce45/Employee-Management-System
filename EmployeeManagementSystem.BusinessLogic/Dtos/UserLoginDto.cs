using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.BusinessLogic.Dtos
{
    public class UserLoginDto
    {   
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        public bool RememberMe { get; set; }=false;
    }
}