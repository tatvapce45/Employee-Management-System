using Moq;
using Xunit;
using EmployeeManagementSystem.BusinessLogic.Services.Implementations;
using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.DataAccess.Results;

namespace EmployeeManagementSystem.Tests.ServiceTests
{
    public class TokenServiceTests
    {
        private readonly Mock<IJwtTokenGeneratorHelper> _jwtHelperMock;
        private readonly Mock<IGenericRepository<Refreshtoken>> _refreshTokenRepoMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<IEmployeesRepository> _employeesRepositoryMock;
        private readonly ITokenService _tokenService;

        public TokenServiceTests()
        {
            _jwtHelperMock = new Mock<IJwtTokenGeneratorHelper>();
            _refreshTokenRepoMock = new Mock<IGenericRepository<Refreshtoken>>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _employeesRepositoryMock = new Mock<IEmployeesRepository>();

            _tokenService = new TokenService(
                _jwtHelperMock.Object,
                _refreshTokenRepoMock.Object,
                _refreshTokenRepositoryMock.Object,
                _employeesRepositoryMock.Object
            );
        }

        private static Employee GetTestEmployee()
        {
            return new Employee
            {
                Id = 1,
                Email = "test@example.com",
                Name = "Test Employee",
                RoleId=1
            };
        }

        private static Refreshtoken GetTestRefreshToken()
        {
            return new Refreshtoken
            {
                Token = "valid-refresh-token",
                Expires = DateTime.Now.AddDays(7),
                EmployeeId = 1,
                IsUsed = false,
                IsRevoked = false,
            };
        }

        [Fact]
        public async Task GenerateTokens_ValidEmployee_ReturnsTokens()
        {
            var employee = GetTestEmployee();
            var accessToken = "valid-access-token";

            _jwtHelperMock.Setup(helper => helper.GenerateJWT(It.IsAny<Employee>(), It.IsAny<int>()))
                          .Returns(accessToken);

            _refreshTokenRepoMock.Setup(repo => repo.AddAsync(It.IsAny<Refreshtoken>()))
                                 .ReturnsAsync(new RepositoryResult<Refreshtoken> { Success = true });

            var result = await _tokenService.GenerateTokens(employee);

            Assert.True(result.Success);
            Assert.Equal(accessToken, result.Data!.GetType().GetProperty("AccessToken")!.GetValue(result.Data));
            Assert.NotNull(result.Data!.GetType().GetProperty("RefreshToken")!.GetValue(result.Data));
        }

        [Fact]
        public async Task GenerateTokens_FailedToCreateRefreshToken_ThrowsException()
        {
            var employee = GetTestEmployee();
            var accessToken = "valid-access-token";
            _jwtHelperMock.Setup(helper => helper.GenerateJWT(It.IsAny<Employee>(), It.IsAny<int>())).Returns(accessToken);

            _refreshTokenRepoMock.Setup(repo => repo.AddAsync(It.IsAny<Refreshtoken>()))
                .ReturnsAsync(new RepositoryResult<Refreshtoken> { Success = false, ErrorMessage = "Database error" });

            var exception = await Assert.ThrowsAsync<Exception>(() => _tokenService.GenerateTokens(employee));
            Assert.Equal("Failed to create refresh token: Database error", exception.Message);
        }

        [Fact]
        public async Task RefreshAccessToken_ValidToken_ReturnsNewTokens()
        {
            var refreshToken = "valid-refresh-token";
            var tokenFromDb = new Refreshtoken
            {
                Token = refreshToken,
                Expires = DateTime.Now.AddDays(7),
                IsUsed = false,
                IsRevoked = false,
                Employee = new Employee
                {
                    Id = 1,
                    Email = "test@example.com",
                    Name = "Test Employee"
                }
            };

            var employee = tokenFromDb.Employee;
            var newAccessToken = "new-access-token";

            var expectedNewRefreshToken = Guid.NewGuid().ToString(); 

            _refreshTokenRepositoryMock.Setup(repo => repo.GetRefreshtokenByToken(refreshToken))
                .ReturnsAsync(tokenFromDb);

            _employeesRepositoryMock.Setup(repo => repo.GetEmployeeByEmail(employee.Email))
                .ReturnsAsync(employee);

            _jwtHelperMock.Setup(helper => helper.GenerateJWT(It.IsAny<Employee>(), It.IsAny<int>()))
                .Returns(newAccessToken);

            _refreshTokenRepoMock.Setup(repo => repo.AddAsync(It.IsAny<Refreshtoken>()))
                .ReturnsAsync(new RepositoryResult<Refreshtoken>
                {
                    Success = true,
                    Data = new Refreshtoken { Token = expectedNewRefreshToken }
                });

            var result = await _tokenService.RefreshAccessToken(refreshToken);

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(newAccessToken, result.Data!.AccessToken);
            Assert.Matches(@"^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", result.Data.RefreshToken); 
        }

        [Fact]
        public async Task RefreshAccessToken_ExpiredToken_ReturnsNull()
        {
            var expiredToken = "expired-refresh-token";
            var expiredRefreshToken = GetTestRefreshToken();
            expiredRefreshToken.Expires = DateTime.Now.AddDays(-1);

            _refreshTokenRepositoryMock.Setup(repo => repo.GetRefreshtokenByToken(expiredToken))
                .ReturnsAsync(expiredRefreshToken);

            var result = await _tokenService.RefreshAccessToken(expiredToken);
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshAccessToken_RevokedToken_ReturnsNull()
        {
            var revokedToken = "revoked-refresh-token";
            var revokedRefreshToken = GetTestRefreshToken();
            revokedRefreshToken.IsRevoked = true;

            _refreshTokenRepositoryMock.Setup(repo => repo.GetRefreshtokenByToken(revokedToken))
                .ReturnsAsync(revokedRefreshToken);
            var result = await _tokenService.RefreshAccessToken(revokedToken);
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshAccessToken_UsedToken_ReturnsNull()
        {
            var usedToken = "used-refresh-token";
            var usedRefreshToken = GetTestRefreshToken();
            usedRefreshToken.IsUsed = true;

            _refreshTokenRepositoryMock.Setup(repo => repo.GetRefreshtokenByToken(usedToken))
                .ReturnsAsync(usedRefreshToken);
            var result = await _tokenService.RefreshAccessToken(usedToken);
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshAccessToken_InvalidToken_ReturnsNull()
        {
            var invalidToken = "invalid-refresh-token";
            _refreshTokenRepositoryMock.Setup(repo => repo.GetRefreshtokenByToken(invalidToken))
                .ReturnsAsync((Refreshtoken?)null);
            var result = await _tokenService.RefreshAccessToken(invalidToken);
            Assert.Null(result);
        }

        [Fact]
        public async Task RefreshAccessToken_NullOrEmptyToken_ReturnsNull()
        {
            var resultWithNull = await _tokenService.RefreshAccessToken(null);
            var resultWithEmpty = await _tokenService.RefreshAccessToken(string.Empty);
            Assert.Null(resultWithNull);
            Assert.Null(resultWithEmpty);
        }

    }
}
