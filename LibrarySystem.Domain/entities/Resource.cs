using LibrarySystem.Domain.Common;

namespace LibrarySystem.Domain.Entities
{
    public abstract class Resource : BaseEntity
    {
        public string Type { get; protected set; } = null!; // Item, ItemSet, Media

        public int? OwnerId { get; set; }

        private readonly List<Value> _values = new();
        public virtual IReadOnlyCollection<Value> Values => _values.AsReadOnly();
    }
}