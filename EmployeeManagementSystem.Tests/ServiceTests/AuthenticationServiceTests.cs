using AutoMapper;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Results;
using EmployeeManagementSystem.BusinessLogic.Services.Implementations;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace EmployeeManagementSystem.Tests.ServiceTests
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<IGenericRepository<Employee>> _mockGenericRepo = new();
        private readonly Mock<IEmployeesRepository> _mockEmployeesRepo = new();
        private readonly Mock<IRolesRepository> _mockRolesRepo = new();
        private readonly Mock<ICountriesRepository> _mockCountriesRepo = new();
        private readonly Mock<IStatesRepository> _mockStatesRepo = new();
        private readonly Mock<ICitiesRepository> _mockCitiesRepo = new();
        private readonly Mock<ITokenService> _mockTokenService = new();
        private readonly Mock<IHashHelper> _mockHashHelper = new();
        private readonly Mock<IEmailSender> _mockEmailSender = new();
        private readonly Mock<IMemoryCache> _mockMemoryCache = new();
        private readonly Mock<IMapper> _mockMapper = new();

        private readonly AuthenticationService _authService;

        public AuthenticationServiceTests()
        {
            _authService = new AuthenticationService(
                _mockGenericRepo.Object,
                _mockEmployeesRepo.Object,
                _mockRolesRepo.Object,
                _mockCountriesRepo.Object,
                _mockStatesRepo.Object,
                _mockCitiesRepo.Object,
                _mockTokenService.Object,
                _mockHashHelper.Object,
                _mockEmailSender.Object,
                _mockMemoryCache.Object,
                _mockMapper.Object
            );
        }
        [Fact]
        public async Task Login_UserNotFound_ReturnsNotFound()
        {
            _mockEmployeesRepo.Setup(repo => repo.GetEmployeeByEmail(It.IsAny<string>()))
                .ReturnsAsync((Employee?)null);

            var dto = new UserLoginDto { Email = "test@example.com", Password = "password" };
            var result = await _authService.Login(dto);
            Assert.False(result.Success);
            Assert.Equal("User with this email does not exist.", result.Message);
        }

        [Fact]
        public async Task Login_IncorrectPassword_ReturnsBadRequest()
        {
            var employee = new Employee { Email = "test@example.com", Password = "encryptedPassword" };
            _mockEmployeesRepo.Setup(repo => repo.GetEmployeeByEmail(It.IsAny<string>()))
                .ReturnsAsync(employee);

            _mockHashHelper.Setup(helper => helper.Decrypt(employee.Password))
                .Returns("correctPassword");

            var dto = new UserLoginDto { Email = "test@example.com", Password = "wrongPassword" };
            var result = await _authService.Login(dto);
            Assert.False(result.Success);
            Assert.Equal("You have entered an incorrect password.", result.Message);
        }

        [Fact]
        public async Task Login_CorrectPassword_SendsOtpAndReturnsOk()
        {
            var employee = new Employee { Email = "test@example.com", Password = "encryptedPassword" };
            _mockEmployeesRepo.Setup(repo => repo.GetEmployeeByEmail(employee.Email))
                .ReturnsAsync(employee);

            _mockHashHelper.Setup(helper => helper.Decrypt(employee.Password))
                .Returns("correctPassword");

            var dto = new UserLoginDto { Email = employee.Email, Password = "correctPassword" };

            var mockCacheEntry = new Mock<ICacheEntry>();
            object? actualKey = null;

            _mockMemoryCache.Setup(mc => mc.CreateEntry(It.IsAny<object>()))
                .Callback<object>(key => actualKey = key)
                .Returns(mockCacheEntry.Object);

            _mockEmailSender.Setup(es => es.SendAsync(
                employee.Email,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var result = await _authService.Login(dto);

            Assert.True(result.Success);
            Assert.Equal("OTP sent to your email.", result.Message);
            Assert.Equal($"OTP_{employee.Email}", actualKey);
        }


        [Fact]
        public async Task Login_ExceptionThrown_ReturnsInternalError()
        {
            _mockEmployeesRepo.Setup(repo => repo.GetEmployeeByEmail(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Database error"));

            var dto = new UserLoginDto { Email = "test@example.com", Password = "password" };
            var result = await _authService.Login(dto);
            Assert.False(result.Success);
            Assert.Equal("An unexpected error occurred during login.", result.Message);
            Assert.NotNull(result.Exception);
            Assert.Equal("Database error", result.Exception.Message);
        }
    }

}