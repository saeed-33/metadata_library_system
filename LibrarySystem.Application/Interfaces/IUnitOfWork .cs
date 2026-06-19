using LibrarySystem.Domain.entities;
using LibrarySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        IGenericRepository<Bookmark> Bookmarks { get; }

        IGenericRepository<SystemUser> SystemUsers { get; }

        IGenericRepository<SystemSetting> SystemSettings { get; }
        IGenericRepository<ItemCopy> ItemCopies { get; }
        IGenericRepository<Patron> Patrons { get; }
        IGenericRepository<BorrowRecord> BorrowRecords { get; }

        Task<bool> RestoreOrUpdateTemplatePropertyAsync(
     int templateId, int propertyId,
     bool isRequired, int displayOrder, string? alternateLabel);

        Task<bool> RestoreBookmarkIfDeletedAsync(string userId, int itemId);

        Task<int> SaveChangesAsync();
    }
}
