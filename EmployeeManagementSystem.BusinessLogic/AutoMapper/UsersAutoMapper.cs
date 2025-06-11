using AutoMapper;
using EmployeeManagementSystem.BusinessLogic.Dtos;
using EmployeeManagementSystem.DataAccess.Models;

namespace EmployeeManagementSystem.BusinessLogic.AutoMapper
{
    public class UsersAutoMapper : Profile
    {
        public UsersAutoMapper()
        {
            CreateMap<UserRegistrationDto, User>()
                .ForMember(dest=>dest.UpdatedAt,opt=>opt.Ignore())
                .ForMember(dest=>dest.CreatedAt,opt=>opt.MapFrom(src=>DateTime.Now));
        }
    }
}
