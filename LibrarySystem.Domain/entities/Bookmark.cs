using LibrarySystem.Domain.Common;
using LibrarySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySystem.Domain.entities
{
    public class Bookmark : BaseEntity
    {

        // ربط مع ASP.NET Identity User
        public string UserId { get; set; } = string.Empty;

        // ربط مع جدول الـ ITEM (بناءً على الـ ERD الخاص بك)
        public int ItemId { get; set; }
        public Item? Item { get; set; } // Navigation Property
    }
}
