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

            modelBuilder.Entity<TemplatePropertyModel>().HasKey(tp => new { tp.TemplateId, tp.PropertyId });

            // Linking Resource to SystemUser (The Library Profile)
            modelBuilder.Entity<ResourceModel>()
                .HasOne(r => r.Owner)
                .WithMany(u => u.OwnedResources)
                .HasForeignKey(r => r.OwnerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Soft Delete Filters
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BasePersistenceModel).IsAssignableFrom(entityType.ClrType) && entityType.BaseType == null)
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var filter = Expression.Lambda(Expression.Equal(
                        Expression.Property(parameter, nameof(BasePersistenceModel.IsDeleted)),
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
            foreach (var entry in ChangeTracker.Entries<BasePersistenceModel>())
            {
                if (entry.State == EntityState.Added) entry.Entity.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified) entry.Entity.ModifiedAt = DateTime.UtcNow;
            }
            return await base.SaveChangesAsync(ct);
        }
    }

}