using Microsoft.AspNetCore.Mvc;
using NomadicBook.Parameters;
using NomadicBook.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NomadicBook.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TradeController : ControllerBase
    {
        private readonly ITradeService TradeService;
        /// <summary>
        /// 注入服務
        /// </summary>
        /// <param name="tradeService">服務</param>
        public TradeController(ITradeService tradeService)
        {
            TradeService = tradeService;
        }
        /// <summary>
        /// 取得指定城市地區的I郵箱資訊
        /// </summary>
        /// <param name="address">含城市和地區的物件</param>
        /// <returns>回傳Status Code，200是成功取得指定區域的I郵箱資訊</returns>
        // GET: api/<TradeController>
        [HttpGet("mailbox")]
        public ActionResult Get([FromQuery]string city,string area)
        {
            var mailBoxList = TradeService.GetMailBox(city, area);
            if (mailBoxList != null)
            {
                return Ok(mailBoxList);
            }
            return NotFound("資料庫無資料");
        }
        /// <summary>
        /// 取得台灣所有縣市
        /// </summary>
        /// <returns>回傳Status Code，200是成功取得，裡面包含城市字串集合</returns>
        // GET: api/<TradeController>
        [HttpGet("address/city")]
        public ActionResult Get()
        {
            var cities = TradeService.GetCity();
            if (cities != null)
            {
                return Ok(cities);
            }
            return NotFound("資料庫無資料");
        }
        /// <summary>
        /// 取得指定縣市的所有鄉鎮市區
        /// </summary>
        /// <param name="city">指定縣市</param>
        /// <returns>回傳Status Code，200是成功取得，裡面包含鄉鎮市區字串集合</returns>
        // GET: api/<TradeController>
        [HttpGet("address/area")]
        public ActionResult Get([FromQuery]string city)
        {
            var areas = TradeService.GetArea(city);
            if (areas != null)
            {
                return Ok(areas);
            }
            return NotFound("資料庫無資料");
        }
        /// <summary>
        /// 取得指定縣市及鄉鎮市區的所有路
        /// </summary>
        /// <param name="city">指定縣市</param>
        /// <param name="area">鄉鎮市區</param>
        /// <returns>回傳Status Code，200是成功取得，裡面包含路的字串集合</returns>
        // GET: api/<TradeController>
        [HttpGet("address/road")]
        public ActionResult GetRoad([FromQuery] string city, string area)
        {
            var roads = TradeService.GetRoad(city, area);
            if (roads != null)
            {
                return Ok(roads);
            }
            return NotFound("資料庫無資料");
        }
        /// <summary>
        /// 將7-11所有門市資料匯入資料庫
        /// </summary>
        /// <returns>回傳Status Code</returns>
        // GET: api/<TradeController> 
        [HttpGet("store/all")]
        public ActionResult GetAllStore()
        {
            return Ok(TradeService.GetAllStore());
        }
        /// <summary>
        /// 取得7-11的所有城市資料
        /// </summary>
        /// <returns>回傳Status Code+城市資料</returns>
        [HttpGet("store/city")]
        public ActionResult GetStoreCity()
        {
            var cities = TradeService.GetStoreCity();
            if (cities != null)
            {
                return Ok(cities);
            }
            return NotFound("資料庫無資料");
        }
        /// <summary>
        /// 取得7-11指定城市的所有區域資料
        /// </summary>
        /// <param name="city">指定城市</param>
        /// <returns>回傳Status Code+區域資料</returns>
        [HttpGet("store/area")]
        public ActionResult GetStoreArea([FromQuery] string city)
        {
            var areas = TradeService.GetStoreArea(city);
            if (areas != null)
            {
                return Ok(areas);
            }
            return NotFound("資料庫無資料");
        }
        /// <summary>
        /// 取得7-11指定城市及區域的我有店家資料
        /// </summary>
        /// <param name="city">指定城市</param>
        /// <param name="area">指定區域</param>
        /// <returns>回傳Status Code+店家資料</returns>
        [HttpGet("store/address")]
        public ActionResult GetStoreAddress([FromQuery] string city,string area)
        {
            var storesList = TradeService.GetStoreAddress(city, area);
            if (storesList != null)
            {
                return Ok(storesList);
            }
            return NotFound("資料庫無資料");
        }
    }
}
