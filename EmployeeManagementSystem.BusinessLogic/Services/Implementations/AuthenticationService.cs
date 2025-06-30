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
    public class AuthenticationService(IGenericRepository<Employee> genericEmployeeRepository,IEmployeesRepository employeesRepository,IRolesRepository rolesRepository,ICountriesRepository countriesRepository,IStatesRepository statesRepository,ICitiesRepository citiesRepository, TokenService tokenService, HashHelper hashHelper, EmailSender emailSender, IMemoryCache memoryCache, IMapper mapper) : IAuthenticationService
    {
        private readonly IGenericRepository<Employee> _genericEmployeeRepository = genericEmployeeRepository;
        private readonly IEmployeesRepository _employeesRepository=employeesRepository;
        private readonly IRolesRepository _rolesRepository=rolesRepository;
        private readonly ICountriesRepository _countriesRepository=countriesRepository;
        private readonly IStatesRepository _statesRepository=statesRepository;
        private readonly ICitiesRepository _citiesRepository=citiesRepository;
        private readonly TokenService _tokenService = tokenService;
        private readonly IMapper _mapper = mapper;
        private readonly HashHelper _hashHelper = hashHelper;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly EmailSender _emailSender = emailSender;

        public async Task<ServiceResult<UserRegistrationDto>> RegisterUser(UserRegistrationDto userRegistrationDto)
        {
            try
            {
                Employee? existingEmployee = await _employeesRepository.GetEmployeeByEmail(userRegistrationDto.Email);
                if (existingEmployee != null)
                {
                    return ServiceResult<UserRegistrationDto>.BadRequest("User with this email already exists.");
                }
                userRegistrationDto.Password = _hashHelper.Encrypt(userRegistrationDto.Password);
                Employee employee = _mapper.Map<Employee>(userRegistrationDto);
                RepositoryResult<Employee> result = await _genericEmployeeRepository.AddAsync(employee);
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
                Employee? existingUser = await _employeesRepository.GetEmployeeByEmail(userLoginDto.Email);
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
            var employee = await _employeesRepository.GetEmployeeByEmail(email);
            if (employee == null)
            {
                return ServiceResult<TokensDto>.NotFound("User not found.");
            }
            var tokens = await _tokenService.GenerateTokens(employee);
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

        public async Task<ServiceResult<RolesResponseDto>> GetRoles()
        {
            try
            {
                List<Role> roles=await _rolesRepository.GetRoles();
                if(roles.Count==0)
                {
                    return ServiceResult<RolesResponseDto>.NotFound("Roles not found!");
                }
                List<RolesDto> rolesDtos=_mapper.Map<List<RolesDto>>(roles);
                RolesResponseDto rolesResponseDto=new()
                {
                    RolesDtos=rolesDtos
                };
                return ServiceResult<RolesResponseDto>.Ok(rolesResponseDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<RolesResponseDto>.InternalError("Unexpected error occurred.",ex);
            }
        }

        public async Task<ServiceResult<CountriesResponseDto>> GetCountries()
        {
            try
            {
                List<Country> countries=await _countriesRepository.GetCountries();
                if(countries.Count==0)
                {
                    return ServiceResult<CountriesResponseDto>.NotFound("Countries not found!");
                }
                List<CountriesDto> countriesDtos=_mapper.Map<List<CountriesDto>>(countries);
                CountriesResponseDto countriesResponseDto=new()
                {
                    CountriesDtos=countriesDtos
                };
                return ServiceResult<CountriesResponseDto>.Ok(countriesResponseDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<CountriesResponseDto>.InternalError("Unexpected error occurred.",ex);
            }
        }

        public async Task<ServiceResult<StatesResponseDto>> GetStatesByCountryId(int countryId)
        {
            try
            {
                List<State1> states=await _statesRepository.GetStatesbyCountryId(countryId);
                if(states.Count==0)
                {
                    return ServiceResult<StatesResponseDto>.NotFound("states not found!");
                }
                List<StatesDto> statesDtos=_mapper.Map<List<StatesDto>>(states);
                StatesResponseDto statesResponseDto=new()
                {
                    StatesDtos=statesDtos
                };
                return ServiceResult<StatesResponseDto>.Ok(statesResponseDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<StatesResponseDto>.InternalError("Unexpected error occurred.",ex);
            }
        }

        public async Task<ServiceResult<CitiesResponseDto>> GetCitiesByStateId(int stateId)
        {
            try
            {
                List<City> cities=await _citiesRepository.GetCitiesByStateId(stateId);
                if(cities.Count==0)
                {
                    return ServiceResult<CitiesResponseDto>.NotFound("Cities not found!");
                }
                List<CitiesDto> citiesDtos=_mapper.Map<List<CitiesDto>>(cities);
                CitiesResponseDto citiesResponseDto=new()
                {
                    CitiesDtos=citiesDtos
                };
                return ServiceResult<CitiesResponseDto>.Ok(citiesResponseDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<CitiesResponseDto>.InternalError("Unexpected error occurred.",ex);
            }
        }
    }
}
