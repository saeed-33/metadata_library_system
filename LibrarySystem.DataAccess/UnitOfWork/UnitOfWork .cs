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

namespace LibrarySystem.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly List<ISyncGeneratedKeys> _repositoriesWithGeneratedKeys = new();

        public IGenericRepository<Vocabulary> Vocabularies { get; }
        public IGenericRepository<Property> Properties { get; }
        public IGenericRepository<ResourceTemplate> ResourceTemplates { get; }
        public IGenericRepository<TemplateProperty> TemplateProperties { get; }
        public IGenericRepository<Resource> Resources { get; }
        public IGenericRepository<Item> Items { get; }
        public IGenericRepository<Media> Medias { get; }
        public IGenericRepository<ItemSet> ItemSets { get; }
        public IGenericRepository<Value> Values { get; }
        public IGenericRepository<SystemUser> SystemUsers { get; }

        public UnitOfWork(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;

            Vocabularies = CreateRepository<Vocabulary, VocabularyModel>(mapper);
            Properties = CreateRepository<Property, PropertyModel>(mapper);
            ResourceTemplates = CreateRepository<ResourceTemplate, ResourceTemplateModel>(mapper);
            TemplateProperties = CreateRepository<TemplateProperty, TemplatePropertyModel>(mapper);
            Resources = CreateRepository<Resource, ResourceModel>(mapper);
            Items = CreateRepository<Item, ItemModel>(mapper);
            Medias = CreateRepository<Media, MediaModel>(mapper);
            ItemSets = CreateRepository<ItemSet, ItemSetModel>(mapper);
            Values = CreateRepository<Value, ValueModel>(mapper);
            SystemUsers = CreateRepository<SystemUser, SystemUserModel>(mapper);
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
