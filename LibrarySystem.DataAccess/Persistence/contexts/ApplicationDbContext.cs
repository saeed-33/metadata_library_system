using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.Domain.common;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibrarySystem.DataAccess.Persistence.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // الموديلات الأساسية
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
        public DbSet<BookmarkModel> Bookmarks => Set<BookmarkModel>();

        // الموديلات الجديدة (تم تصحيح طريقة التعريف لتناسب بقية الكود)
        public DbSet<SystemSettingModel> SystemSettings => Set<SystemSettingModel>();
        public DbSet<ItemCopyModel> ItemCopies => Set<ItemCopyModel>();
        public DbSet<PatronModel> Patrons => Set<PatronModel>();
        public DbSet<BorrowRecordModel> BorrowRecords => Set<BorrowRecordModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // تعيين أسماء الجداول (لتجنب جمع الأسماء تلقائياً بشكل خاطئ)
            modelBuilder.Entity<ResourceModel>().ToTable("Resources");
            modelBuilder.Entity<ItemModel>().ToTable("Items");
            modelBuilder.Entity<MediaModel>().ToTable("Media");
            modelBuilder.Entity<ItemSetModel>().ToTable("ItemSets");
            modelBuilder.Entity<BookmarkModel>().ToTable("Bookmarks");

            // جداول الإعارة الجديدة
            modelBuilder.Entity<SystemSettingModel>().ToTable("SystemSettings");
            modelBuilder.Entity<ItemCopyModel>().ToTable("ItemCopies");
            modelBuilder.Entity<PatronModel>().ToTable("Patrons");
            modelBuilder.Entity<BorrowRecordModel>().ToTable("BorrowRecords");

            // إعدادات الـ Many-to-Many بين المجموعات والعناصر
            modelBuilder.Entity<ItemSetModel>()
     .HasMany(itemSet => itemSet.Items) // هذا هو السطر الذي كان ينقصك
     .WithMany(item => item.ItemSets)
     .UsingEntity<Dictionary<string, object>>(
         "ItemSetItems",
         right => right.HasOne<ItemModel>().WithMany().HasForeignKey("ItemId").OnDelete(DeleteBehavior.Restrict),
         left => left.HasOne<ItemSetModel>().WithMany().HasForeignKey("ItemSetId").OnDelete(DeleteBehavior.Cascade),
         join =>
         {
             join.HasKey("ItemSetId", "ItemId");
             join.ToTable("ItemSetItems");
         });

            // مفاتيح العلاقات (Existing)
            modelBuilder.Entity<TemplatePropertyModel>().HasKey(tp => new { tp.TemplateId, tp.PropertyId });

            // ==========================================
            // إعدادات نظام الإعارة والإعدادات (تحديث)
            // ==========================================

            // 1. إعدادات النسخ (ItemCopies)
            modelBuilder.Entity<ItemCopyModel>()
                .HasIndex(c => c.Barcode)
                .IsUnique();

            modelBuilder.Entity<ItemCopyModel>()
                .HasOne(c => c.Item)
                .WithMany(i => i.Copies) // تأكد من وجود ICollection<ItemCopyModel> Copies في ItemModel
                .HasForeignKey(c => c.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. إعدادات المستعيرين (Patrons)
            modelBuilder.Entity<PatronModel>()
                .HasIndex(p => p.NationalId)
                .IsUnique();

            // 3. إعدادات سجل الإعارة (BorrowRecords)
            modelBuilder.Entity<BorrowRecordModel>()
                .HasOne(b => b.Patron)
                .WithMany()
                .HasForeignKey(b => b.PatronId)
                .OnDelete(DeleteBehavior.Restrict);

            // الربط المفقود: علاقة سجل الإعارة بالنسخة
            modelBuilder.Entity<BorrowRecordModel>()
                .HasOne(b => b.Copy)
                .WithMany()
                .HasForeignKey(b => b.CopyId)
                .OnDelete(DeleteBehavior.Restrict);

            // 4. إعدادات النظام (SystemSettings)
            modelBuilder.Entity<SystemSettingModel>()
                .HasIndex(s => s.Key)
                .IsUnique();

            // ==========================================
            // بقية العلاقات القديمة
            // ==========================================

            modelBuilder.Entity<PropertyModel>()
                .HasOne(p => p.Vocabulary).WithMany(v => v.Properties)
                .HasForeignKey(p => p.VocabularyId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MediaModel>()
                .HasOne(m => m.Item).WithMany(i => i.Medias)
                .HasForeignKey(m => m.ItemId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ValueModel>()
                .HasOne(v => v.Resource).WithMany(r => r.Values)
                .HasForeignKey(v => v.ResourceId).OnDelete(DeleteBehavior.Cascade);

            // Soft Delete Filters (تلقائي لكل الكيانات التي تدعم ISoftDelete)
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
            foreach (var entry in ChangeTracker.Entries<BasePersistenceModel>())
            {
                if (entry.State == EntityState.Added) entry.Entity.CreatedAt = DateTime.UtcNow;
                else if (entry.State == EntityState.Modified) entry.Entity.ModifiedAt = DateTime.UtcNow;
            }
            return await base.SaveChangesAsync(ct);
        }
    }
}