using System;
using System.Collections.Generic;

#nullable disable

namespace NomadicBook.Models.db
{
    public partial class Book
    {
        public int BookId { get; set; }
        public short UserId { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Isbn { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public string PublishingHouse { get; set; }
        public string CategoryId { get; set; }
        public DateTime PublishDate { get; set; }
        public double? BookLong { get; set; }
        public double? BookWidth { get; set; }
        public double? BookHigh { get; set; }
        public string Experience { get; set; }
        public DateTime? ExperienceDay { get; set; }
        public string Introduction { get; set; }
        public byte ConditionNum { get; set; }
        public string Condition { get; set; }
        public bool BookStatus { get; set; }
        public bool BookExist { get; set; }
        public string StoreAddress { get; set; }
        public string StoreName { get; set; }
        public string MailBoxAddress { get; set; }
        public string MailBoxName { get; set; }
        public string HomeAddress { get; set; }
        public string FaceTradeCity { get; set; }
        public string FaceTradeArea { get; set; }
        public string FaceTradeRoad { get; set; }
        public string FaceTradePath { get; set; }
        public string FaceTradeDetail { get; set; }
        public string TrueName { get; set; }
        public string CellphoneNumber { get; set; }
    }
}
