using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.Domain.common;

namespace LibrarySystem.DataAccess.Persistence.Contexts
{
   
    public class CustomIdentityDbContext : IdentityDbContext<AppUserModel, AppRoleModel, string>
    {
        public CustomIdentityDbContext(DbContextOptions<CustomIdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppRoleModel>().HasQueryFilter(role => !role.IsDeleted);

            builder.Ignore<SystemUserModel>();
            builder.Ignore<ResourceModel>();
            builder.Ignore<ItemModel>();
            builder.Ignore<MediaModel>();
            builder.Ignore<ItemSetModel>();
            builder.Ignore<ValueModel>();
            builder.Ignore<VocabularyModel>();
            builder.Ignore<PropertyModel>();
            builder.Ignore<ResourceTemplateModel>();
            builder.Ignore<TemplatePropertyModel>();
            // You can customize Identity table names here if you want (e.g. builder.Entity<ApplicationUserModel>().ToTable("Users");)
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                }
            }

            foreach (var entry in ChangeTracker.Entries<IAuditable>())
            {
                if (entry.State == EntityState.Added) entry.Entity.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified) entry.Entity.ModifiedAt = DateTime.UtcNow;
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
