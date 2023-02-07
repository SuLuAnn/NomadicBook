using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Dto
{
    public class ListProductDto
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public string BookPhoto { get; set; }
        public string Condition { get; set; }
        public byte ConditionNum { get; set; }
        
    }
}
