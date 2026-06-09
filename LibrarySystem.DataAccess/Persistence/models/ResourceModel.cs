using LibrarySystem.Domain.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("Resources")]
    public abstract class ResourceModel : BasePersistenceModel
    {
        [Required, StringLength(50)]
        public string Type { get; set; } = string.Empty;

        // The property the error is complaining about:
        public int? OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public SystemUserModel? Owner { get; set; } // This is the 'Owner' definition

        public ICollection<ValueModel> Values { get; set; } = new List<ValueModel>();
    }
}
