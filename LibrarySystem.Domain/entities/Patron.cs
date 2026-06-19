using LibrarySystem.Domain.Common;

namespace LibrarySystem.Domain.Entities
{
  public class Patron : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty; 
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
}
}