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
            return services;
        }

        public static IServiceCollection AddJwtTokenGeneratorHelper(this IServiceCollection services)
        {
            services.AddScoped<JwtTokenGeneratorHelper>();
            return services;
        }
    }
}