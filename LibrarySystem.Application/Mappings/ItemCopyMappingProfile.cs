using AutoMapper;
using LibrarySystem.Application.Commands.ItemCopies;
using LibrarySystem.Application.DTOs.ItemCopies;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Mappings;

public class ItemCopyMappingProfile : Profile
{
    public ItemCopyMappingProfile()
    {
        CreateMap<ItemCopy, ItemCopyResponse>();
        CreateMap<ItemCopy, ItemCopyAdminResponse>();

        // من Commands إلى Entity
        CreateMap<CreateItemCopyCommand, ItemCopy>();
        CreateMap<UpdateItemCopyCommand, ItemCopy>();
    }
}