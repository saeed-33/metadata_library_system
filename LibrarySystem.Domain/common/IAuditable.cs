using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Domain.common
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        string? CreatedBy { get; set; }
        DateTime? ModifiedAt { get; set; }
        string? ModifiedBy { get; set; }
    }
}
