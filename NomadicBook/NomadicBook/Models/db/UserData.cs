using System;
using System.Collections.Generic;

#nullable disable

namespace NomadicBook.Models.db
{
    public partial class UserData
    {
        public short UserId { get; set; }
        public bool Verification { get; set; }
        public string NickName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string UserPhoto { get; set; }
        public string SelfIntroduction { get; set; }
        public double TotalEvaluation { get; set; }
        public int TradeNum { get; set; }
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
