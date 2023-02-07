using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Dto
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public string UserName { get; set; }
        public string UserPhoto { get; set; }
        public string MessageTime { get; set; }
        public string Message { get; set; }
        public bool IsOwner { get; set; }
    }
}
