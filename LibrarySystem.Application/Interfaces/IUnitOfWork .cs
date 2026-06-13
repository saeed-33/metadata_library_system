using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Vocabulary> Vocabularies { get; }
        IGenericRepository<Property> Properties { get; }
        IGenericRepository<ResourceTemplate> ResourceTemplates { get; }
        IGenericRepository<TemplateProperty> TemplateProperties { get; }
        IGenericRepository<Resource> Resources { get; }
        IGenericRepository<Item> Items { get; }
        IGenericRepository<Media> Medias { get; }
        IItemSetRepository ItemSets { get; }
        IGenericRepository<Value> Values { get; }


        IGenericRepository<SystemUser> SystemUsers { get; }

        Task<bool> RestoreOrUpdateTemplatePropertyAsync(
     int templateId, int propertyId,
     bool isRequired, int displayOrder, string? alternateLabel);


        Task<int> SaveChangesAsync();
    }
}
