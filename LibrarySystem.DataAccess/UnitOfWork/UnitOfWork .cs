using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.DataAccess.Persistence.Contexts;
using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.Domain.entities;
using LibrarySystem.Domain.Entities; 
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly List<ISyncGeneratedKeys> _repositoriesWithGeneratedKeys = new();

        public IGenericRepository<Vocabulary> Vocabularies { get; }
        public IGenericRepository<Property> Properties { get; }
        public IGenericRepository<ResourceTemplate> ResourceTemplates { get; }
        public IGenericRepository<TemplateProperty> TemplateProperties { get; }
        public IGenericRepository<Resource> Resources { get; }
        public IGenericRepository<Item> Items { get; }
        public IGenericRepository<Media> Medias { get; }
        public IItemSetRepository ItemSets { get; }
        public IGenericRepository<Value> Values { get; }
        public IGenericRepository<SystemUser> SystemUsers { get; }
        public IGenericRepository<Bookmark> Bookmarks { get; }

        // ---- الإضافات الجديدة لنظام الإعارة ----
        public IGenericRepository<SystemSetting> SystemSettings { get; }
        public IGenericRepository<ItemCopy> ItemCopies { get; }
        public IGenericRepository<Patron> Patrons { get; }
        public IGenericRepository<BorrowRecord> BorrowRecords { get; }

        public UnitOfWork(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

            // تهيئة المستودعات الموجودة مسبقاً
            Vocabularies = CreateRepository<Vocabulary, VocabularyModel>(mapper);
            Properties = CreateRepository<Property, PropertyModel>(mapper);
            ResourceTemplates = CreateRepository<ResourceTemplate, ResourceTemplateModel>(mapper);
            TemplateProperties = CreateRepository<TemplateProperty, TemplatePropertyModel>(mapper);
            Resources = CreateRepository<Resource, ResourceModel>(mapper);
            Items = CreateRepository<Item, ItemModel>(mapper);
            Medias = CreateRepository<Media, MediaModel>(mapper);

            var itemSetRepository = new ItemSetRepository(_context, mapper);
            ItemSets = itemSetRepository;
            _repositoriesWithGeneratedKeys.Add(itemSetRepository);

            Values = CreateRepository<Value, ValueModel>(mapper);
            SystemUsers = CreateRepository<SystemUser, SystemUserModel>(mapper);
            Bookmarks = CreateRepository<Bookmark, BookmarkModel>(mapper);

            // ---- تهيئة المستودعات الجديدة لنظام الإعارة ----
            SystemSettings = CreateRepository<SystemSetting, SystemSettingModel>(mapper);
            ItemCopies = CreateRepository<ItemCopy, ItemCopyModel>(mapper);
            Patrons = CreateRepository<Patron, PatronModel>(mapper);
            BorrowRecords = CreateRepository<BorrowRecord, BorrowRecordModel>(mapper);
        }

        public async Task<bool> RestoreBookmarkIfDeletedAsync(string userId, int itemId)
        {
            var model = await _context.Bookmarks
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(b =>
                    b.UserId == userId &&
                    b.ItemId == itemId &&
                    b.IsDeleted == true);

            if (model == null) return false;

            model.IsDeleted = false;
            model.DeletedAt = null;
            return true;
        }

        public async Task<bool> RestoreOrUpdateTemplatePropertyAsync(
            int templateId, int propertyId,
            bool isRequired, int displayOrder, string? alternateLabel)
        {
            var model = await _context.TemplateProperties
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(tp =>
                    tp.TemplateId == templateId &&
                    tp.PropertyId == propertyId);

            if (model == null) return false;

            model.IsDeleted = false;
            model.DeletedAt = null;
            model.IsRequired = isRequired;
            model.DisplayOrder = displayOrder;
            model.AlternateLabel = alternateLabel;

            return true;
        }

        public async Task<int> SaveChangesAsync()
        {
            var result = await _context.SaveChangesAsync();
            foreach (var repository in _repositoriesWithGeneratedKeys)
            {
                repository.SyncGeneratedKeys();
            }
            return result;
        }

        public void Dispose() => _context.Dispose();

        //Factory Method Pattern

        private IGenericRepository<TDomain> CreateRepository<TDomain, TPersistence>(IMapper mapper)
            where TDomain : class
            where TPersistence : class
        {
            var repository = new MappedRepository<TDomain, TPersistence>(_context, mapper);
            _repositoriesWithGeneratedKeys.Add(repository);
            return repository;
        }
    }
}