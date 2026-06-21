using AutoMapper;
using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.Domain.entities;
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

            CreateMap<Resource, ResourceModel>()
                .ReverseMap()
                .ForMember(dest => dest.Values, opt => opt.Ignore());

            // تحديث Item ليشمل تجاهل Copies عند التحويل العكسي لمنع تكرار البيانات غير الضرورية
            CreateMap<Item, ItemModel>()
                .ForMember(dest => dest.ItemSets, opt => opt.Ignore())
                .ForMember(dest => dest.Copies, opt => opt.MapFrom(src => src.Copies)) // ربط النسخ
                .ReverseMap()
                .ForMember(dest => dest.ItemSets, opt => opt.Ignore())
                .ForMember(dest => dest.Values, opt => opt.Ignore())
                .ForMember(dest => dest.Copies, opt => opt.Ignore()); // تجاهل النسخ عند التحويل من موديل لكيان لضمان التحديث عبر الـ Handler

            CreateMap<Media, MediaModel>()
                .ReverseMap()
                .ForMember(dest => dest.Values, opt => opt.Ignore());
            
            CreateMap<Bookmark, BookmarkModel>().ReverseMap();

            CreateMap<ItemSet, ItemSetModel>()
                .ReverseMap()
                .ForMember(dest => dest.Values, opt => opt.Ignore())
                .ForMember(dest => dest.Items, opt => opt.Ignore());
            
            CreateMap<Value, ValueModel>()
                .ForMember(dest => dest.Resource, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Resource, opt => opt.Ignore());

            // ==========================================
            // التعديلات الجديدة لنظام الإعارة والإعدادات
            // ==========================================
            
            // 1. إعدادات النظام
            CreateMap<SystemSetting, SystemSettingModel>().ReverseMap();

            // 2. المستعيرون
            CreateMap<Patron, PatronModel>().ReverseMap();

            // 3. نسخ العناصر
            // لا نتجاهل Item هنا لأن استعلامات الإعارة تحتاج الوصول إلى عنوان/معرف العنصر الأب.
            CreateMap<ItemCopy, ItemCopyModel>().ReverseMap().MaxDepth(2);

            // 4. سجل الإعارة
            // لا نتجاهل Copy/Patron هنا لأن DTOs الإعارة تعتمد على الباركود واسم المستعير.
            CreateMap<BorrowRecord, BorrowRecordModel>().ReverseMap().MaxDepth(2);

            // ==========================================

            CreateMap<SystemUser, SystemUserModel>()
                .ForMember(dest => dest.OwnedResources, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Roles, opt => opt.Ignore())
                .ForMember(dest => dest.OwnedResources, opt => opt.Ignore());
        }
    }
}