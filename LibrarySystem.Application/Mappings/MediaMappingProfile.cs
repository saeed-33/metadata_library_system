using AutoMapper;
using LibrarySystem.Application.DTOs.Media;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Mappings;

public class MediaMappingProfile : Profile
{
    public MediaMappingProfile()
    {
        CreateMap<LibrarySystem.Domain.Entities.Media, MediaResponse>()
            .ForMember(dest => dest.MetadataValues, opt => opt.MapFrom(src => src.Values));
    }
}