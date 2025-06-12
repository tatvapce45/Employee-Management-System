using AutoMapper;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.BusinessLogic.Mappers
{
    public class EmployeeAutoMapper : Profile
    {
        public EmployeeAutoMapper()
        {
            CreateMap<CreateEmployeeDto, Employee>()
                .ForMember(dest => dest.HiringDate, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<Employee, EmployeeDto>();

            CreateMap<UpdateEmployeeDto, Employee>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())          
                .ForMember(dest => dest.HiringDate, opt => opt.Ignore())  
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));  
        }
    }
}
