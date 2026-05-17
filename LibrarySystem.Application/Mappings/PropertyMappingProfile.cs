using AutoMapper;
using LibrarySystem.Application.Commands.Properties;
using LibrarySystem.Application.DTOs.Properties;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Mappings;

public class PropertyMappingProfile : Profile
{
    public PropertyMappingProfile()
    {
        CreateMap<Property, PropertyResponse>()
            .ForMember(dest => dest.VocabularyPrefix, opt => opt.MapFrom(src => src.Vocabulary!.Prefix));

        // تحويل من Commands إلى Entity
        CreateMap<CreatePropertyCommand, Property>();
        CreateMap<UpdatePropertyCommand, Property>();
    }
}