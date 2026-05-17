using AutoMapper;
using LibrarySystem.Application.Commands.ItemSets;
using LibrarySystem.Application.DTOs.ItemSets;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Mappings;

public class ItemSetMappingProfile : Profile
{
    public ItemSetMappingProfile()
    {
        CreateMap<ItemSet, ItemSetResponse>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner == null ? null : src.Owner.FullName));

        CreateMap<Item, ItemSetItemResponse>();

        CreateMap<CreateItemSetCommand, ItemSet>();
        CreateMap<UpdateItemSetCommand, ItemSet>();
    }
}
