using System;
using System.Collections.Generic;

#nullable disable

namespace NomadicBook.Models.db
{
    public partial class SeekRecord
    {
        public int SeekId { get; set; }
        public short SeekUserId { get; set; }
        public DateTime SeekDate { get; set; }
        public int SeekBookId { get; set; }
        public string SeekName { get; set; }
        public string SeekCellphone { get; set; }
        public bool SeekSend { get; set; }
        public bool SeekReceive { get; set; }
        public byte TradeMode { get; set; }
        public string SeekToAddress { get; set; }
        public string SeekToName { get; set; }
        public double? SeekEvaluation { get; set; }
        public short SeekedUserId { get; set; }
        public DateTime? SeekedDate { get; set; }
        public int? SeekedBookId { get; set; }
        public byte SeekStatus { get; set; }
        public string SeekedToAddress { get; set; }
        public string SeekedToName { get; set; }
        public string SeekedName { get; set; }
        public string SeekedCellphone { get; set; }
        public bool SeekedSend { get; set; }
        public bool SeekedReceive { get; set; }
        public double? SeekedEvaluation { get; set; }
    }
}
