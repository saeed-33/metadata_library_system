using LibrarySystem.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Domain.Entities
{
    public class Media : Resource
    {
        public Media() => Type = "Media";
        public int ItemId { get; set; }
        public virtual Item Item { get; set; } = null!;
        public string StoragePath { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string? MimeType { get; set; } // jpeg, pdf,...
        public long FileSize { get; set; }
        public string? AltText { get; set; } // description for the media object
    }
}