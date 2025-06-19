namespace EmployeeManagementSystem.BusinessLogic.Dtos
{
    public class ChangePasswordDto
    {
        public int Id { get; set; }

        public required string CurrentPassword { get; set; }

        public required string NewPassword { get; set; }

        public required string ConfirmNewPassword { get; set; }
    }

}