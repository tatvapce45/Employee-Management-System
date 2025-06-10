using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.BusinessLogic.DtoValidators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeManagementSystem.BusinessLogic
{
    public static class ValidationServiceCollection
    {
        public static IServiceCollection AddValidationServices(this IServiceCollection services)
        {
            services.AddTransient<IValidator<UserRegistrationDto>, UserRegistrationDtoValidator>();
            services.AddTransient<IValidator<UserLoginDto>,UserLoginDtoValidator>();
            services.AddTransient<IValidator<CreateEmployeeDto>,CreateEmployeeValidator>();
            services.AddTransient<IValidator<UpdateEmployeeDto>,UpdateEmployeeValidator>();
            return services;
        }
    }
}
