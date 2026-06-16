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

        CreateMap<TemplateProperty, TemplatePropertyResponse>()
            .ForMember(dest => dest.PropertyLabel, opt => opt.MapFrom(src => src.Property!.Label));

        CreateMap<ResourceTemplate, ResourceTemplateAdminResponse>()
           .ForMember(dest => dest.Properties, opt => opt.MapFrom(src => src.TemplateProperties));

        CreateMap<TemplateProperty, TemplatePropertyAdminResponse>()
            .ForMember(dest => dest.PropertyLabel, opt => opt.MapFrom(src => src.Property!.Label));

        CreateMap<CreateResourceTemplateCommand, ResourceTemplate>();
        CreateMap<UpdateResourceTemplateCommand, ResourceTemplate>();

        // ← this was missing
        CreateMap<TemplatePropertyRequest, TemplateProperty>()
            .ForMember(dest => dest.TemplateId, opt => opt.Ignore())
            .ForMember(dest => dest.Template, opt => opt.Ignore())
            .ForMember(dest => dest.Property, opt => opt.Ignore())
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
    }
}