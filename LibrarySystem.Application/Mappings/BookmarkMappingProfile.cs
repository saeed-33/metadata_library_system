using AutoMapper;
using LibrarySystem.Application.DTOs.Bookmarks;
using LibrarySystem.Domain.entities;
namespace LibrarySystem.Application.Mappings;
public class BookmarkMappingProfile : Profile
{
    public BookmarkMappingProfile()
    {
        CreateMap<Bookmark, BookmarksResponse>();
    }
}