using System;
using System.Collections.Generic;

#nullable disable

namespace NomadicBook.Models.db
{
    public partial class RoomMessage
    {
        public int MessageId { get; set; }
        public int SeekId { get; set; }
        public short UserId { get; set; }
        public DateTime MessageTime { get; set; }
        public string Message { get; set; }
    }
}
