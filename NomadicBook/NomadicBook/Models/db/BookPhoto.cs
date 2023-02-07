using System;
using System.Collections.Generic;

#nullable disable

namespace NomadicBook.Models.db
{
    public partial class BookPhoto
    {
        public int BookPhotoId { get; set; }
        public int BookId { get; set; }
        public string BookPhoto1 { get; set; }
    }
}
