using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Dto
{
    public class BasicUserDto
    {
        public string NickName { get; set; }
        public string Email { get; set; }
        public string UserPhoto { get; set; }
        public string SelfIntroduction { get; set; }
        public double Evaluation { get; set; }
        public int TradeNum { get; set; }
    }
}
