using NomadicBook.Dto;
using NomadicBook.Models.db;
using NomadicBook.Parameters;
using NomadicBook.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Service
{
    public class TradeService: ITradeService
    {
        private readonly NomadicBookContext NomadicBookContext;
        private readonly IReptile Reptile;
        /// <summary>
        /// 注入資料庫
        /// </summary>
        /// <param name="nomadicBookContext">資料庫</param>
        public TradeService(NomadicBookContext nomadicBookContext, IReptile reptile)
        {
            NomadicBookContext = nomadicBookContext;
            Reptile = reptile;
        }
        /// <summary>
        /// 取得指定城市地區的I郵箱資訊
        /// </summary>
        /// <param name="address">含城市和地區的物件</param>
        /// <returns>I郵箱資訊集合</returns>
        public List<IMailBoxDto> GetMailBox(string city, string area)
        {
            var mailBoxes = NomadicBookContext.Imailboxes.Select(mailbox => mailbox);
            if (city != null)
            {
                mailBoxes=mailBoxes.Where(mailbox => mailbox.MailboxCity.Equals(city));
            }
            if (area != null)
            {
                mailBoxes=mailBoxes.Where(mailbox => mailbox.MailboxArea.Equals(area));
            }
            var mailBoxList = mailBoxes.Select(mailbox => new IMailBoxDto
            {
                MailboxAddress = mailbox.MailboxAddress,
                MailboxName = mailbox.MailboxName
            }).ToList();
            return mailBoxList;
        }
        /// <summary>
        /// 取得台灣所有縣市
        /// </summary>
        /// <returns>回傳縣市字串集合</returns>
        public List<string> GetCity() 
        {
            return NomadicBookContext.Addresses.Select(address => address.City).Distinct().ToList();
        }

        /// <summary>
        /// 取得指定縣市所有鄉鎮市
        /// </summary>
        /// <param name="city">指定的縣市</param>
        /// <returns>回傳鄉鎮市字串集合</returns>
        public List<string> GetArea(string city) 
        {
            return NomadicBookContext.Addresses.Where(address => address.City.Equals(city)).Select(address => address.Area).Distinct().ToList();
        }
        /// <summary>
        ///  取得指定縣市及鄉鎮市區的所有路
        /// </summary>
        /// <param name="city">指定的縣市</param>
        /// <param name="area">指定的鄉鎮市區</param>
        /// <returns>回傳路的字串集合</returns>
        public List<string> GetRoad(string city, string area) 
        {
            return NomadicBookContext.Addresses.Where(address => city.Equals(address.City) && area.Equals(address.Area))
                .Select(address => address.Road).Distinct().ToList();
        }
        /// <summary>
        /// 將7-11所有門市資料匯入資料庫
        /// </summary>
        /// <returns>資料庫變動數量</returns>
        public int GetAllStore()
        {
            var stores = Reptile.GetStore();
            NomadicBookContext.AddRange(stores);
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 取得7-11的所有城市資料
        /// </summary>
        /// <returns>城市資料</returns>
        public List<string> GetStoreCity()
        {
            return NomadicBookContext.ConvenienceStores.Select(store=>store.ShopCity).Distinct().ToList();
        }
        /// <summary>
        /// 取得7-11指定城市的所有區域資料
        /// </summary>
        /// <param name="city">指定城市</param>
        /// <returns>區域資料</returns>
        public List<string> GetStoreArea(string city)
        {
            return NomadicBookContext.ConvenienceStores.Where(store=>store.ShopCity.Equals(city)).Select(store => store.ShopArea).Distinct().ToList();
        }
        /// <summary>
        /// 取得7-11指定城市及區域的我有店家資料
        /// </summary>
        /// <param name="city">指定城市</param>
        /// <param name="area">指定區域</param>
        /// <returns>店家資料</returns>
        public List<StoreDto> GetStoreAddress(string city,string area)
        {
            var store = NomadicBookContext.ConvenienceStores.Select(store => store);
            if (city != null)
            {
                store = store.Where(store => store.ShopCity.Equals(city));
            }
            if (area != null)
            {
                store = store.Where(store => store.ShopArea.Equals(area));
            }
            var storeList = store.Select(store => new StoreDto
            {
                ShopName=store.ShopName,
                ShopAddress=store.ShopAddress
            }).ToList();
            return storeList;
        }
    }
}
