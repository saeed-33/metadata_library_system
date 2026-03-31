using LibrarySystem.Domain.Common;

namespace LibrarySystem.Domain.Entities
{
    public class ItemSet : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<ItemSetMember> ItemSetMembers { get; set; } = new List<ItemSetMember>();
    }
}