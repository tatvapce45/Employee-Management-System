namespace EmployeeManagementSystem.BusinessLogic.Dtos
{
    public class TokenRefreshResponseDto
    {
        public required string RefreshToken { get; set; }

        public required string AccessToken { get; set; }
    }
}