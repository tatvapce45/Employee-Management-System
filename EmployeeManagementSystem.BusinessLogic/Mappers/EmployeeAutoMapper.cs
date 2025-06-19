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

            CreateMap<UserRegistrationDto, Employee>()
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
        ;

            CreateMap<Employee, UserRegistrationDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.Split(new char[] { ' ' }, 2, StringSplitOptions.None)[0]))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.Contains(' ')
    ? src.Name.Split(new char[] { ' ' }, 2, StringSplitOptions.None)[1]
    : ""));

            CreateMap<UpdateProfileDto, Employee>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                    .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.Now));

            CreateMap<Employee, UpdateProfileDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name.Split(new char[] { ' ' }, 2, StringSplitOptions.None)[0]))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Name.Contains(' ')
    ? src.Name.Split(new char[] { ' ' }, 2, StringSplitOptions.None)[1]
    : ""))
                .ForMember(dest => dest.UserName, opt => opt.Ignore()); 

        }
    }
}
