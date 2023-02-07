using System;
using System.Collections.Generic;

#nullable disable

namespace NomadicBook.Models.db
{
    public partial class Isbn
    {
        public string Isbn1 { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public string PublishingHouse { get; set; }
        public string CategoryId { get; set; }
        public string PublishDate { get; set; }
        public double BookLong { get; set; }
        public double BookWidth { get; set; }
        public double BookHigh { get; set; }
        public string Introduction { get; set; }
    }
}
