using AutoMapper;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.BusinessLogic.Mappers
{
    public class StatesAutoMapper : Profile
    {
        public StatesAutoMapper()
        {
            CreateMap<State, StatesDto>();
        }
    }
}
