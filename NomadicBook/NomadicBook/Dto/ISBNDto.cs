using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Dto
{
    public class ISBNDto
    {
        public string PublishDate { get; set; }
        public string Isbn { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public string PublishingHouse { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public double BookLong { get; set; }
        public double BookWidth { get; set; }
        public double BookHigh { get; set; }
        public string Introduction { get; set; }

    }
}
