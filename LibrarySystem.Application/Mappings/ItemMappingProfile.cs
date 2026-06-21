using AutoMapper;
using LibrarySystem.Application.Commands.Items;
using LibrarySystem.Application.DTOs.Items;
using LibrarySystem.Application.DTOs.Values;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Mappings;
public class ItemMappingProfile : Profile
{
    public ItemMappingProfile()
    {
        CreateMap<Item, ItemResponse>()
            .ForMember(dest => dest.OwnerName,
                opt => opt.MapFrom(src => src.Owner != null ? src.Owner.FullName : null))
            .ForMember(dest => dest.MetadataValues,
                opt => opt.MapFrom(src => src.Values));

        CreateMap<Value, ItemValueResponse>()
            .ForMember(dest => dest.PropertyLabel,
                opt => opt.MapFrom(src => src.Property != null ? src.Property.Label : string.Empty));


        CreateMap<Item, ItemAdminResponse>()
                  .ForMember(dest => dest.OwnerName,
                      opt => opt.MapFrom(src => src.Owner != null ? src.Owner.FullName : null))
                  .ForMember(dest => dest.MetadataValues,
                      opt => opt.MapFrom(src => src.Values));

        CreateMap<Value, ItemAdminValueResponse>()
            .ForMember(dest => dest.PropertyLabel,
                opt => opt.MapFrom(src => src.Property != null ? src.Property.Label : string.Empty));

        // Commands → Entity
        CreateMap<CreateItemCommand, Item>()
            .ForMember(dest => dest.Values, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.Ignore());

        CreateMap<UpdateItemCommand, Item>()
            .ForMember(dest => dest.Values, opt => opt.Ignore())
            .ForMember(dest => dest.Type, opt => opt.Ignore());

        CreateMap<CreateValueRequest, Value>();
    }
}