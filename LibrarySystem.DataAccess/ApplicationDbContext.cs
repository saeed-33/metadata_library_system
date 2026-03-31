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

            // 1. Composite Key for TemplateProperty
            modelBuilder.Entity<TemplateProperty>()
                .HasKey(tp => new { tp.TemplateId, tp.PropertyId });

            // 2. Composite Key for ItemSetMember
            modelBuilder.Entity<ItemSetMember>()
                .HasKey(ism => new { ism.ItemId, ism.ItemSetId });

            // 3. FIX: Explicitly define the main relationship for ItemValues
            modelBuilder.Entity<ItemValue>()
                .HasOne(iv => iv.Item)             // ItemValue has one parent Item
                .WithMany(i => i.ItemValues)      // Item has many ItemValues
                .HasForeignKey(iv => iv.ItemId)   // Using ItemId as the key
                .OnDelete(DeleteBehavior.Cascade); // If Item is deleted, delete its values

            // 4. FIX: Explicitly define the Internal Linking relationship
            modelBuilder.Entity<ItemValue>()
                .HasOne(iv => iv.LinkedItem)      // ItemValue can have one LinkedItem
                .WithMany()                       // LinkedItem does NOT need a collection of values pointing to it
                .HasForeignKey(iv => iv.LinkedItemId)
                .OnDelete(DeleteBehavior.Restrict); // Do NOT delete the linked item if the value is deleted
        }
    }
}