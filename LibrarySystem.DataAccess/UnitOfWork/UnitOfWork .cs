using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibrarySystem.Application.Interfaces;
using LibrarySystem.DataAccess.Persistence.Contexts;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

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

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            Vocabularies = new GenericRepository<Vocabulary>(context);
            Properties = new GenericRepository<Property>(context);
            ResourceTemplates = new GenericRepository<ResourceTemplate>(context);
            TemplateProperties = new GenericRepository<TemplateProperty>(context);
            Resources = new GenericRepository<Resource>(context);
            Items = new GenericRepository<Item>(context);
            Medias = new GenericRepository<Media>(context);
            ItemSets = new GenericRepository<ItemSet>(context);
            Values = new GenericRepository<Value>(context);
            SystemUsers = new GenericRepository<SystemUser>(context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}