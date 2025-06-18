using AutoMapper;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.BusinessLogic.Mappers
{
    public class RolesAutoMapper : Profile
    {
        public RolesAutoMapper()
        {
            CreateMap<Role, RolesDto>();
        }
    }
}
