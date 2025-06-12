using AutoMapper;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.Helpers;
using EmployeeManagementSystem.BusinessLogic.Results;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using EmployeeManagementSystem.DataAccess.Results;
using Microsoft.Extensions.Caching.Memory;

namespace EmployeeManagementSystem.BusinessLogic.Services.Implementations
{
    public class AuthenticationService(IGenericRepository<User> genericUserRepository, IUsersRepository usersRepository, TokenService tokenService, HashHelper hashHelper, EmailSender emailSender, IMemoryCache memoryCache, IMapper mapper) : IAuthenticationService
    {
        private readonly IGenericRepository<User> _genericUserRepository = genericUserRepository;
        private readonly IUsersRepository _usersRepository = usersRepository;
        private readonly TokenService _tokenService = tokenService;
        private readonly IMapper _mapper = mapper;
        private readonly HashHelper _hashHelper = hashHelper;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly EmailSender _emailSender = emailSender;

        public async Task<ServiceResult<UserRegistrationDto>> RegisterUser(UserRegistrationDto userRegistrationDto)
        {
            try
            {
                User? existingUser = await _usersRepository.GetUserByEmail(userRegistrationDto.Email);
                if (existingUser != null)
                {
                    return ServiceResult<UserRegistrationDto>.BadRequest("User with this email already exists.");
                }
                userRegistrationDto.Password = _hashHelper.Encrypt(userRegistrationDto.Password);
                User user = _mapper.Map<User>(userRegistrationDto);
                RepositoryResult<User> result = await _genericUserRepository.AddAsync(user);
                if (!result.Success)
                {
                    return ServiceResult<UserRegistrationDto>.InternalError($"Failed to register user: {result.ErrorMessage}");
                }
                return ServiceResult<UserRegistrationDto>.Created(userRegistrationDto, "User registered successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<UserRegistrationDto>.InternalError("An unexpected error occurred during registration.", ex);
            }
        }

        public async Task<ServiceResult<string>> Login(UserLoginDto userLoginDto)
        {
            try
            {
                User? existingUser = await _usersRepository.GetUserByEmail(userLoginDto.Email);
                if (existingUser == null)
                {
                    return ServiceResult<string>.NotFound("User with this email does not exist.");
                }
                string correctPassword = _hashHelper.Decrypt(existingUser.Password);
                if (correctPassword != userLoginDto.Password)
                {
                    return ServiceResult<string>.BadRequest("You have entered an incorrect password.");
                }
                string otpCode = new Random().Next(100000, 999999).ToString();
                _memoryCache.Set($"OTP_{existingUser.Email}", otpCode, TimeSpan.FromMinutes(5));
                string htmlBody = $"<p>Your OTP code is: <strong>{otpCode}</strong></p>";
                await _emailSender.SendAsync(existingUser.Email, "Your OTP Code", otpCode, htmlBody);
                return ServiceResult<string>.Ok(null,"OTP sent to your email.");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.InternalError("An unexpected error occurred during login.", ex);
            }
        }

        public async Task<ServiceResult<TokensDto>> VerifyOtp(string email, string submittedOtp)
        {
            if (!_memoryCache.TryGetValue($"OTP_{email}", out string? cachedOtp))
            {
                return ServiceResult<TokensDto>.BadRequest("OTP expired or not found.");
            }
            if (cachedOtp != submittedOtp)
            {
                return ServiceResult<TokensDto>.BadRequest("Invalid OTP.");
            }
            var user = await _usersRepository.GetUserByEmail(email);
            if (user == null)
            {
                return ServiceResult<TokensDto>.NotFound("User not found.");
            }
            var tokens = await _tokenService.GenerateTokens(user);
            var accessToken = tokens.Data?.GetType().GetProperty("AccessToken")?.GetValue(tokens.Data, null)?.ToString();
            var refreshToken = tokens.Data?.GetType().GetProperty("RefreshToken")?.GetValue(tokens.Data, null)?.ToString();
            if (accessToken != null && refreshToken != null)
            {
                _memoryCache.Remove($"OTP_{email}");
                TokensDto tokensDto = new()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
                return ServiceResult<TokensDto>.Ok(tokensDto, "Login successful.");
            }
            return ServiceResult<TokensDto>.InternalError("Failed to generate tokens after OTP verification.");
        }
    }
}
