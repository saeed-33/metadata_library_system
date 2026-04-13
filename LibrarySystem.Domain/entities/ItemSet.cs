using LibrarySystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Domain.Entities
{
    public class ItemSet : Resource
    {
        public ItemSet() => Type = "ItemSet";
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsPublic { get; set; }

        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}