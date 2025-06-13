using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using EmployeeManagementSystem.DataAccess.Models;
using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
namespace EmployeeManagementSystem.BusinessLogic.Helpers
{
    public class JwtTokenGeneratorHelper(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public static string FetchEmail(string Token)
        {
            if (string.IsNullOrEmpty(Token))
            {
                return string.Empty;
            }
            var tokenEmailClaim = new JwtSecurityTokenHandler().ReadJwtToken(Token).Claims.FirstOrDefault(claim => claim.Type == "mail");
            var tokenEmail = tokenEmailClaim?.Value ?? string.Empty;
            return tokenEmail;
        }

        public string GenerateJWT(User user, int expireMinutes = 15)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userClaims = new List<Claim>
            {
                new("userName", user.UserName!),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role.Name),
                new("mail", user.Email),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.Now.AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
