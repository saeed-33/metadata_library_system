using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using LibrarySystem.Application.Interfaces;
using LibrarySystem.DataAccess.Persistence.Contexts;
using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

        public UnitOfWork(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

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
        }

        public async Task<bool> RestoreOrUpdateTemplatePropertyAsync(
    int templateId, int propertyId,
    bool isRequired, int displayOrder, string? alternateLabel)
        {
            // Find the row including soft-deleted ones
            var model = await _context.TemplateProperties
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(tp =>
                    tp.TemplateId == templateId &&
                    tp.PropertyId == propertyId);

            if (model == null) return false;

            // Update the ALREADY TRACKED model directly — no new object created
            // EF sees the same object it already tracks → no conflict
            model.IsDeleted = false;
            model.DeletedAt = null;
            model.IsRequired = isRequired;
            model.DisplayOrder = displayOrder;
            model.AlternateLabel = alternateLabel;

            // No need to call Update() — EF already tracks this object
            // It will detect the changes automatically on SaveChangesAsync

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