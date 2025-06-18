using AutoMapper;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.BusinessLogic.Mappers
{
    public class CountriesAutoMapper : Profile
    {
        public CountriesAutoMapper()
        {
            CreateMap<Country, CountriesDto>();
        }
    }
}
