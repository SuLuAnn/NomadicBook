using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Dto
{
    public class MatchDto
    {
        public int SeekId { get; set; }

        public byte TradeMode { get; set; }
        public MatchDetailDto Seek { get; set; }
        public MatchDetailDto Seeked { get; set; }
    }
}
