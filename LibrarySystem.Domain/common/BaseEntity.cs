using LibrarySystem.Domain.common;

namespace LibrarySystem.Domain.Common
{
    public abstract class BaseEntity : ISoftDelete
    {
        public int Id { get; protected set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}