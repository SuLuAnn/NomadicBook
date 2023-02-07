using NomadicBook.Dto;
using NomadicBook.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Service
{
    public interface ITradeService
    {
        public List<IMailBoxDto> GetMailBox(string city, string area);
        public List<string> GetCity();
        public List<string> GetArea(string city);
        public List<string> GetRoad(string city, string area);
        public int GetAllStore();
        public List<string> GetStoreCity();
        public List<string> GetStoreArea(string city);
        public List<StoreDto> GetStoreAddress(string city, string area);
    }
}
