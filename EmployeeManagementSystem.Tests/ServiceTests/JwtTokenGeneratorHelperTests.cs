using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using EmployeeManagementSystem.BusinessLogic.Helpers;
using EmployeeManagementSystem.DataAccess.Models;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagementSystem.Tests.ServiceTests
{
    public class JwtTokenGeneratorHelperTests
    {
        private const string SecretKey = "12345678901234567890123456789012"; 
        private const string Issuer = "TestIssuer";
        private const string Audience = "TestAudience";

        private readonly Mock<IConfiguration> _mockConfig;
        private readonly JwtTokenGeneratorHelper _tokenHelper;

        public JwtTokenGeneratorHelperTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["Jwt:Key"]).Returns(SecretKey);
            _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns(Issuer);
            _mockConfig.Setup(c => c["Jwt:Audience"]).Returns(Audience);
            _tokenHelper = new JwtTokenGeneratorHelper(_mockConfig.Object);
        }

        [Fact]
        public void FetchEmail_ValidTokenWithMailClaim_ReturnsEmail()
        {
            var token = GenerateTokenWithMail("test@example.com");
            var email = JwtTokenGeneratorHelper.FetchEmail(token);
            Assert.Equal("test@example.com", email);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void FetchEmail_NullOrEmptyToken_ReturnsEmpty(string? token)
        {
            var result = JwtTokenGeneratorHelper.FetchEmail(token);
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void FetchEmail_NoMailClaim_ReturnsEmpty()
        {
            var claims = new List<Claim> { new Claim("userName", "Alice") };
            var token = GenerateToken(claims);

            var result = JwtTokenGeneratorHelper.FetchEmail(token);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GenerateJWT_ValidEmployee_ReturnsValidToken()
        {
            var employee = new Employee
            {
                Id = 1,
                Name = "Alice",
                Email = "alice@example.com",
                Role = new Role { Name = "Admin" }
            };

            var token = _tokenHelper.GenerateJWT(employee);

            Assert.False(string.IsNullOrWhiteSpace(token));

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            Assert.Contains(jwt.Claims, c => c.Type == "userName" && c.Value == "Alice");
            Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == "1");
            Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            Assert.Contains(jwt.Claims, c => c.Type == "mail" && c.Value == "alice@example.com");
        }

        private static string GenerateTokenWithMail(string email)
        {
            var claims = new List<Claim> { new Claim("mail", email) };
            return GenerateToken(claims);
        }

        private static string GenerateToken(IEnumerable<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}