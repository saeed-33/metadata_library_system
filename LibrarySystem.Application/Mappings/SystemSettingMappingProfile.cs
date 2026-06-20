using AutoMapper;
using LibrarySystem.Application.Commands.SystemSettings;
using LibrarySystem.Application.DTOs.SystemSettings;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Mappings;

public class SystemSettingMappingProfile : Profile
{
    public SystemSettingMappingProfile()
    {
        CreateMap<SystemSetting, SystemSettingResponse>();
        CreateMap<SystemSetting, SystemSettingAdminResponse>();

        // من Command التحديث إلى Entity (Upsert logic)
        CreateMap<UpdateSystemSettingCommand, SystemSetting>();
    }
}