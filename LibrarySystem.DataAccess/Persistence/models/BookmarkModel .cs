using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("Bookmarks")]
    public class BookmarkModel : BasePersistenceModel
    {
        [Required]
        [StringLength(450)] // 450 هو الطول القياسي لـ Id في ASP.NET Identity
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int ItemId { get; set; }

        // Navigation Property (لربط المفضلة بالـ Item)
        [ForeignKey(nameof(ItemId))]
        public virtual ItemModel? Item { get; set; }
    }
}
