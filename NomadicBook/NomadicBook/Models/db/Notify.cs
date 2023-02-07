using System;
using System.Collections.Generic;

#nullable disable

namespace NomadicBook.Models.db
{
    public partial class Notify
    {
        public int NotifyId { get; set; }
        public short UserId { get; set; }
        public DateTime NotifyDate { get; set; }
        public string Notify1 { get; set; }
        public bool Clicked { get; set; }
    }
}
