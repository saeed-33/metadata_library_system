using LibrarySystem.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Domain.Entities
{
    public class Item : Resource
    {
        public Item() => Type = "Item";
        public int? TemplateId { get; set; }
        public virtual ResourceTemplate? Template { get; set; }

        public virtual ICollection<Media> Medias { get; set; } = new List<Media>();
        public virtual ICollection<ItemSet> ItemSets { get; set; } = new List<ItemSet>();
    }
}