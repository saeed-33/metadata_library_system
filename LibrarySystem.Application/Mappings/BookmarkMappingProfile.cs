using AutoMapper;
using LibrarySystem.Application.DTOs.Users;
using LibrarySystem.Domain.entities;

namespace LibrarySystem.Application.Mappings;

public class BookmarkMappingProfile : Profile
{
    public BookmarkMappingProfile()
    {
        CreateMap<Bookmark, BookmarksResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ItemId));
    }
}
