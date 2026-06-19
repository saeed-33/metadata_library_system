using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.DataAccess.Persistence.models
{
    [Table("SystemSettings")]

    public class SystemSettingModel : BasePersistenceModel
    {
          [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [Required]
    public string Value { get; set; } = string.Empty;
    }
}
