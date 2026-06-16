using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Application.DTOs
{
    public record VocabularyAdminResponse(
        int Id,
        string Prefix,
        string NamespaceUri,
        string Label,
        bool IsDeleted // <--- الإضافة هنا

    );
}
