using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Dto
{
    public class MatchDetailDto
    {
        public short SeekUserId { get; set; }
        public string SeekUserName { get; set; }
        public double? Evaluation { get; set; }
        public int TradeNum { get; set; }
        public string SeekDate { get; set; }
        public int SeekBookId { get; set; }
        public string SeekBookName { get; set; }
        public string SeekCondition { get; set; }
        public byte SeekConditionNum { get; set; }
        public string SeekAuthor { get; set; }
        public string SeekBookPhoto { get; set; }
        public string SeekName { get; set; }
        public string SeekCellphone { get; set; }
        public bool SeekSend { get; set; }
        public bool SeekReceive { get; set; }
        public string SeekToAddress { get; set; }
        public string SeekToName { get; set; }
    }
}
