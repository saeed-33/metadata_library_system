using LibrarySystem.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibrarySystem.Domain.Entities
{
    public class MediaFile : BaseEntity
    {
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public virtual Item? Item { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public string? FileType { get; set; } // Optional
        public long FileSize { get; set; }    // Value type (long) can't be null unless you use long?
        public string? Extension { get; set; }
        public string? Dimensions { get; set; } // Optional (e.g., PDFs don't have px dimensions)
        public string? ThumbnailPath { get; set; } // Optional
    }
}