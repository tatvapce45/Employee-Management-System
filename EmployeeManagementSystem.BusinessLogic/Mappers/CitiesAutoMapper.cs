using AutoMapper;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.BusinessLogic.Mappers
{
    public class CitiesAutoMapper : Profile
    {
        public CitiesAutoMapper()
        {
            CreateMap<City, CitiesDto>();
        }
    }
}
