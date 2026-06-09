using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.Domain.common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibrarySystem.DataAccess.Persistence.Contexts
{
    public class ApplicationDbContext : DbContext // Standard DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<VocabularyModel> Vocabularies => Set<VocabularyModel>();
        public DbSet<PropertyModel> Properties => Set<PropertyModel>();
        public DbSet<ResourceTemplateModel> ResourceTemplates => Set<ResourceTemplateModel>();
        public DbSet<TemplatePropertyModel> TemplateProperties => Set<TemplatePropertyModel>();
        public DbSet<ResourceModel> Resources => Set<ResourceModel>();
        public DbSet<ItemModel> Items => Set<ItemModel>();
        public DbSet<MediaModel> Media => Set<MediaModel>();
        public DbSet<ItemSetModel> ItemSets => Set<ItemSetModel>();
        public DbSet<ValueModel> Values => Set<ValueModel>();
        public DbSet<SystemUserModel> SystemUsers => Set<SystemUserModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // NO base.OnModelCreating here because we aren't using Identity in this context

            modelBuilder.Entity<ResourceModel>().ToTable("Resources");
            modelBuilder.Entity<ItemModel>().ToTable("Items");
            modelBuilder.Entity<MediaModel>().ToTable("Media");
            modelBuilder.Entity<ItemSetModel>().ToTable("ItemSets");

            modelBuilder.Entity<ItemSetModel>()
                .HasMany(itemSet => itemSet.Items)
                .WithMany(item => item.ItemSets)
                .UsingEntity<Dictionary<string, object>>(
                    "ItemSetItems",
                    right => right.HasOne<ItemModel>()
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Restrict),
                    left => left.HasOne<ItemSetModel>()
                        .WithMany()
                        .HasForeignKey("ItemSetId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.HasKey("ItemSetId", "ItemId");
                        join.ToTable("ItemSetItems");
                    });

            modelBuilder.Entity<TemplatePropertyModel>().HasKey(tp => new { tp.TemplateId, tp.PropertyId });

            modelBuilder.Entity<PropertyModel>()
                .HasOne(property => property.Vocabulary)
                .WithMany(vocabulary => vocabulary.Properties)
                .HasForeignKey(property => property.VocabularyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ItemModel>()
                .HasOne(item => item.Template)
                .WithMany()
                .HasForeignKey(item => item.TemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<MediaModel>()
                .HasOne(media => media.Item)
                .WithMany(item => item.Medias)
                .HasForeignKey(media => media.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ValueModel>()
                .HasOne(value => value.Resource)
                .WithMany(resource => resource.Values)
                .HasForeignKey(value => value.ResourceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ValueModel>()
                .HasOne(value => value.Property)
                .WithMany()
                .HasForeignKey(value => value.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Linking Resource to SystemUser (The Library Profile)
            modelBuilder.Entity<ResourceModel>()
                .HasOne(r => r.Owner)
                .WithMany(u => u.OwnedResources)
                .HasForeignKey(r => r.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Soft Delete Filters
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType) && entityType.BaseType == null)
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var filter = Expression.Lambda(Expression.Equal(
                        Expression.Property(parameter, nameof(ISoftDelete.IsDeleted)),
                        Expression.Constant(false)), parameter);
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
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
            return await base.SaveChangesAsync(ct);
        }
    }

}
