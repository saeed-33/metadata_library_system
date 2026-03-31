using Microsoft.EntityFrameworkCore;
using LibrarySystem.Domain.Entities;

namespace LibrarySystem.DataAccess // Updated Namespace
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Vocabulary> Vocabularies { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<ResourceTemplate> ResourceTemplates { get; set; }
        public DbSet<TemplateProperty> TemplateProperties { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemValue> ItemValues { get; set; }
        public DbSet<ItemSet> ItemSets { get; set; }
        public DbSet<ItemSetMember> ItemSetMembers { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite Key for TemplateProperty
            modelBuilder.Entity<TemplateProperty>()
                .HasKey(tp => new { tp.TemplateId, tp.PropertyId });

            // Composite Key for ItemSetMember
            modelBuilder.Entity<ItemSetMember>()
                .HasKey(ism => new { ism.ItemId, ism.ItemSetId });

            // Self-Referencing for Internal Linking (Requirement 3-6)
            modelBuilder.Entity<ItemValue>()
                .HasOne(iv => iv.LinkedItem)
                .WithMany()
                .HasForeignKey(iv => iv.LinkedItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}