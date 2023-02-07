using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Dto
{
    public class SignInDto
    {
        public short UserId { get; set; }
        public string NickName { get; set; }

        public string Authenticate { get; set; }
    }
}
