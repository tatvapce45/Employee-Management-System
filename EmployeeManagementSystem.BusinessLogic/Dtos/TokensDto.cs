using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.BusinessLogic.Dtos
{
    public class TokensDto
    {   
        public string AccessToken{get;set;}=string.Empty;

        public string RefreshToken{get;set;}=string.Empty;
    }
}