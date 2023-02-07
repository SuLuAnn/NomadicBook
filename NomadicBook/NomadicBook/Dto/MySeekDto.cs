using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Dto
{
    public class MySeekDto
    {
        public int SeekId { get; set; }
        public int SeekBookId { get; set; }
        public string SeekBookName { get; set; }
        public string SeekBookPhoto { get; set; }
        public string Condition { get; set; }
        public byte ConditionNum { get; set; }
        public string Author { get; set; }
        public bool SeekStatus { get; set; }
        public byte TradeMode { get; set; }
        public string SeekDate { get; set; }
    }
}
