using LibrarySystem.Domain.common;
using LibrarySystem.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Domain.Entities
{
    public abstract class Resource : BaseEntity, IAuditable
    {
        public string Type { get; protected set; } = null!; // Item, ItemSet, Media
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public int? OwnerId { get; set; }

        private readonly List<Value> _values = new();
        public virtual IReadOnlyCollection<Value> Values => _values.AsReadOnly();
    }
}