using AutoMapper;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.BusinessLogic.Mappers
{
    public class DepartmentAutoMapper : Profile
    {
        public DepartmentAutoMapper()
        {
            CreateMap<Department, DepartmrentDto>();

            CreateMap<DepartmrentDto, Department>();

            CreateMap<CreateDepartmentDto, Department>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<UpdateDepartmentDto, Department>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}
