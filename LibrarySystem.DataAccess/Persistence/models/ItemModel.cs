using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("Items")]
    public class ItemModel : ResourceModel
    {
        public int? TemplateId { get; set; }
        public ResourceTemplateModel? Template { get; set; }
        public virtual ICollection<MediaModel> Medias { get; set; } = new List<MediaModel>();
        public virtual ICollection<ItemSetModel> ItemSets { get; set; } = new List<ItemSetModel>();
    }
}
