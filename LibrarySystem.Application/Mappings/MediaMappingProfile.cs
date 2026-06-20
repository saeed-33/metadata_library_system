using AutoMapper;
using LibrarySystem.Application.Commands.Media;
using LibrarySystem.Application.DTOs.Media;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Mappings;

public class MediaMappingProfile : Profile
{
    public MediaMappingProfile()
    {
        CreateMap<LibrarySystem.Domain.Entities.Media, MediaResponse>()
            .ForMember(dest => dest.MetadataValues, opt => opt.MapFrom(src => src.Values));

        CreateMap<LibrarySystem.Domain.Entities.Media, MediaAdminResponse>()
           .ForMember(dest => dest.MetadataValues, opt => opt.MapFrom(src => src.Values));

        // Commands → Entity
        CreateMap<CreateMediaCommand, LibrarySystem.Domain.Entities.Media>()
            .ForMember(dest => dest.Values, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.Ignore());

        CreateMap<UpdateMediaCommand, LibrarySystem.Domain.Entities.Media>()
            .ForMember(dest => dest.Values, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.Ignore());
    }
}