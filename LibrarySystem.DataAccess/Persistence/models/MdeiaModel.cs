using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("Media")]
    public class MediaModel : ResourceModel
    {
        public int ItemId { get; set; }
        public ItemModel? Item { get; set; }
        [Required, StringLength(500)] public string StoragePath { get; set; } = string.Empty;
        [Required, StringLength(255)] public string FileName { get; set; } = string.Empty;
        public string? MimeType { get; set; }
        public long FileSize { get; set; }
        public string? AltText { get; set; }
    }
}
