using NomadicBook.Dto;
using NomadicBook.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Service
{
    public interface IMessageService
    {
        public int AddMessage(MessageParameter message);
        public List<MessageDto> GetMessages(int seekId, short userId);

        public int DeleteMessage(int messageId);

        public int SetMessage(int messageId,string message);
    }
}
