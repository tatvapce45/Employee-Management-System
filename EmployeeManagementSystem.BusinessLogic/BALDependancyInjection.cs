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
            services.AddScoped<TokenService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<HashHelper>();
            services.AddScoped<EmailSender>();
            return services;
        }

        public static IServiceCollection AddJwtTokenGeneratorHelper(this IServiceCollection services)
        {
            services.AddScoped<JwtTokenGeneratorHelper>();
            return services;
        }

        public static IServiceCollection AddAutoMappers(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(EmployeeAutoMapper));
            services.AddAutoMapper(typeof(UsersAutoMapper)); 
            services.AddAutoMapper(typeof(RolesAutoMapper)); 
            services.AddAutoMapper(typeof(CountriesAutoMapper)); 
            services.AddAutoMapper(typeof(StatesAutoMapper)); 
            services.AddAutoMapper(typeof(CitiesAutoMapper)); 
            services.AddAutoMapper(typeof(DepartmentAutoMapper)); 
            return services;
        }
    }
}