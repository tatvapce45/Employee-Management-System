using EmployeeManagementSystem.BusinessLogic.Mappers;
using EmployeeManagementSystem.BusinessLogic.Helpers;
using EmployeeManagementSystem.BusinessLogic.Services.Implementations;
using EmployeeManagementSystem.BusinessLogic.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
namespace EmployeeManagementSystem.BusinessLogic
{
    public static class BALDependancyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IEmployeesService, EmployeesService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IHashHelper,HashHelper>();
            services.AddScoped<NotificationService>();
            services.AddScoped<IEmailSender,EmailSender>();
            return services;
        }

        public static IServiceCollection AddJwtTokenGeneratorHelper(this IServiceCollection services)
        {
            services.AddScoped<IJwtTokenGeneratorHelper,JwtTokenGeneratorHelper>();
            return services;
        }

        public static IServiceCollection AddAutoMappers(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(EmployeeAutoMapper));
            services.AddAutoMapper(typeof(RolesAutoMapper)); 
            services.AddAutoMapper(typeof(CountriesAutoMapper)); 
            services.AddAutoMapper(typeof(StatesAutoMapper)); 
            services.AddAutoMapper(typeof(CitiesAutoMapper)); 
            services.AddAutoMapper(typeof(DepartmentAutoMapper)); 
            services.AddAutoMapper(typeof(AttendanceMapper)); 
            return services;
        }
    }
}