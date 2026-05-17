using AutoMapper;
using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.DataAccess.Mappings
{
    public class PersistenceMappingProfile : Profile
    {
        public PersistenceMappingProfile()
        {
            CreateMap<Vocabulary, VocabularyModel>().ReverseMap();
            CreateMap<Property, PropertyModel>().ReverseMap();
            CreateMap<ResourceTemplate, ResourceTemplateModel>().ReverseMap();
            CreateMap<TemplateProperty, TemplatePropertyModel>().ReverseMap();
            CreateMap<Resource, ResourceModel>().ReverseMap();
            CreateMap<Item, ItemModel>().ReverseMap();
            CreateMap<Media, MediaModel>().ReverseMap();
            CreateMap<ItemSet, ItemSetModel>().ReverseMap();
            CreateMap<Value, ValueModel>().ReverseMap();
            CreateMap<SystemUser, SystemUserModel>().ReverseMap();
        }
    }
}
