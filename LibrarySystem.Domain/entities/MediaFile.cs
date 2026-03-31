using LibrarySystem.Domain.Common;

namespace LibrarySystem.Domain.Entities
{
    public class MediaFile : BaseEntity
    {
        public int ItemId { get; set; }
        public Item Item { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; } // PDF, Image, etc.
        public long FileSize { get; set; }
        public string Extension { get; set; }
        public string Dimensions { get; set; }
        public string ThumbnailPath { get; set; }
    }
}