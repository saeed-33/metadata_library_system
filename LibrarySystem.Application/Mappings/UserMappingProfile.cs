using AutoMapper;
using LibrarySystem.Application.Commands.Users;
using LibrarySystem.Application.DTOs.Users;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<SystemUser, UserResponse>()
            // تحويل قائمة كائنات Role إلى قائمة نصوص (أسماء الأدوار)
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name)));

        CreateMap<CreateSystemUserCommand, SystemUser>();
        CreateMap<UpdateSystemUserCommand, SystemUser>();
    }
}