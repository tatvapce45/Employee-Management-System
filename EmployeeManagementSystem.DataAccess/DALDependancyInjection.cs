using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using EmployeeManagementSystem.DataAccess.Repositories.Implementations;
using EmployeeManagementSystem.DataAccess.Models;
namespace EmployeeManagementSystem.DataAccess
{
    public static class DALDependancyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IEmployeesRepository, EmployeesRepository>();
            services.AddScoped<IDepartmentsRepository, DepartmentsRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRolesRepository, RolesRepository>();
            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IStatesRepository, StatesRepository>();
            services.AddScoped<ICitiesRepository, CitiesRepository>();
            return services;
        }

        public static IServiceCollection AddDbContextConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("AssignedTaskDbConnection");
            services.AddDbContext<EmployeeManagementSystemContext>(q => q.UseNpgsql(conn));
            return services;
        }
    }
}