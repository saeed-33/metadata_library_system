using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("Values")]
    public class ValueModel : BasePersistenceModel
    {
        public int ResourceId { get; set; }
        [ForeignKey("ResourceId")]
        public ResourceModel? Resource { get; set; }

        public int PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        public PropertyModel? Property { get; set; }

        public string? ValueText { get; set; }

        // Add these two properties that were missing:
        [StringLength(255)]
        public string? ValueUri { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = "literal"; // literal, uri, or resource

        [Required]
        [StringLength(10)]
        public string Language { get; set; } = "en";

        public int? ValueResourceId { get; set; } // For internal linking
    }
}
