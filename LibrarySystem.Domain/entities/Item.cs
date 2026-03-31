using LibrarySystem.Domain.Common;

namespace LibrarySystem.Domain.Entities
{
    public class Item : BaseEntity
    {
        public string CreatedBy { get; set; }

        public int TemplateId { get; set; }
        public ResourceTemplate Template { get; set; }

        public ICollection<ItemValue> ItemValues { get; set; } = new List<ItemValue>();
        public ICollection<MediaFile> MediaFiles { get; set; } = new List<MediaFile>();
        public ICollection<ItemSetMember> ItemSetMembers { get; set; } = new List<ItemSetMember>();
    }
}