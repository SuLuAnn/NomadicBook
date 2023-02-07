using NomadicBook.Dto;
using NomadicBook.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Service
{
    public interface INotifyService
    {
        public List<NotifyDto> GetNotify(short userId);
        public int GetNotifyNum(short userId);

        public void SendNotify(short userId,string notify);
        public string GetBookName(int bookId);
        public string GetEmail(short userId);
        public void SendMessager(int bookId, short userId, Global.MessengerNum num);
        public (int, short) GetOtherSideBook(int seekId, short userId);
    }
}
