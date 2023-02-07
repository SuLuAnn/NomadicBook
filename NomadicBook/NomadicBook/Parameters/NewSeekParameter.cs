using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Parameters
{
    public class NewSeekParameter
    {
        public short SeekUserId { get; set; }
        public int SeekBookId { get; set; }
        public byte TradeMode { get; set; }
        public string SeekToAddress { get; set; }
        public string SeekToName { get; set; }
        public short SeekedUserId { get; set; }
    }
}
