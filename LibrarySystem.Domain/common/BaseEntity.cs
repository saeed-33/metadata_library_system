using LibrarySystem.Domain.common;

namespace LibrarySystem.Domain.Common
{
    public abstract class BaseEntity : ISoftDelete
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
