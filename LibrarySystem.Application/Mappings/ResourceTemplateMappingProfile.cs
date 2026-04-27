using AutoMapper;
using LibrarySystem.Application.Commands.ResourceTemplates;
using LibrarySystem.Application.DTOs.ResourceTemplates;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Mappings;

public class ResourceTemplateMappingProfile : Profile
{
    public ResourceTemplateMappingProfile()
    {
        CreateMap<ResourceTemplate, ResourceTemplateResponse>()
            .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.TemplateProperties));

        // تحويل جدول الربط إلى DTO بسيط للعرض
        CreateMap<TemplateProperty, TemplatePropertyResponse>()
            .ForMember(dest => dest.PropertyLabel, opt => opt.MapFrom(src => src.Property!.Label));

        CreateMap<CreateResourceTemplateCommand, ResourceTemplate>();
        CreateMap<UpdateResourceTemplateCommand, ResourceTemplate>();
    }
}