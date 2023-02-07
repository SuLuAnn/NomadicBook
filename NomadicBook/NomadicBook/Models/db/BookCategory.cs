using System;
using System.Collections.Generic;

#nullable disable

namespace NomadicBook.Models.db
{
    public partial class BookCategory
    {
        public string CategoryId { get; set; }
        public byte MainId { get; set; }
        public string BigCategory { get; set; }
        public string DetailCategory { get; set; }
    }
}
