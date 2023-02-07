using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Parameters
{
    public class UserParameter
    {
        public string NickName { get; set; }
        public IFormFile UserPhoto { get; set; }
        public string SelfIntroduction { get; set; }
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
