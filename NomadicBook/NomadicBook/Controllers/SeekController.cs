using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NomadicBook.Parameters;
using NomadicBook.Service;
using NomadicBook.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static NomadicBook.Utils.Global;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NomadicBook.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class SeekController : ControllerBase
    {
        /// <summary>
        /// 注入服務
        /// </summary>
        private readonly ISeekService SeekService;
        private readonly INotifyService NotifyService;
        public SeekController(ISeekService seekService, INotifyService notifyService)
        {
            SeekService = seekService;
            NotifyService = notifyService;
        }
        /// <summary>
        /// 使用者新徵求一本書，要新增一筆邀約資料
        /// </summary>
        /// <param name="newSeek">徵求資料</param>
        /// <returns>回傳Status Code，200成功，404失敗</returns>
        // POST api/<SeekController>
        [HttpPost("new")]
        public ActionResult Post([FromBody] NewSeekParameter newSeek)
        {
            string prompt = Global.IsCorrectLength(newSeek.SeekToAddress, StringLimit.Address);
            if (prompt != null)
            {
                return NotFound(prompt);
            }
            int result = SeekService.AddNewSeek(newSeek);
            if (result > 0) 
            {
                NotifyService.SendMessager(newSeek.SeekBookId, newSeek.SeekedUserId,Global.MessengerNum.Request);
                return Ok("新增成功");
            }
            return NotFound("新增失敗");
            
        }
        /// <summary>
        /// 取得所有尚未被回應的我的徵求的資料
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <returns>回傳Status Code，200成功，並附上徵求紀錄集合，404失敗</returns>
        // GET: api/<SeekController>
        [HttpGet("myself/{userId}")]
        public ActionResult GetMyself(short userId)
        {
            var result = SeekService.GetMySeek(userId);
            if (result.Count() > 0) 
            {
                return Ok(result);
            }
            return NotFound("目前沒有我的徵求紀錄");
            
        }
        /// <summary>
        /// 別人對使用者的徵求，並且使用者尚未回應的邀請
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <returns>回傳Status Code，200成功，並附上徵求紀錄集合，404失敗</returns>
        // GET api/<SeekController>/5
        [HttpGet("otherpeople/{userId}")]
        public ActionResult GetOther(short userId)
        {
            var result = SeekService.GetOtherSeek(userId);
            if (result.Count() > 0)
            {
                return Ok(result);
            }
            return NotFound("目前沒有徵求邀請的紀錄");
        }
        /// <summary>
        /// 在回應別人對自身的徵求時，要選擇一本對方所擁有的書做交換，並且這本書要擁有與對方的徵求所選擇相同的交易方式
        /// </summary>
        /// <param name="userId">對方的使用者id</param>
        /// <param name="tradeMode">交易方式編號</param>
        /// <returns>回傳Status Code，200成功取得對方上架並且相同交易方式的書籍，404為對方沒有上架書籍</returns>
        // GET api/<SeekController>/5
        [HttpGet("otherpeople/books/{userId}/{tradeMode}")]
        public ActionResult GetOther(short userId,byte tradeMode)
        {
            var result = SeekService.GetOtherBook(userId, tradeMode);
            if (result.Count() > 0)
            {
                return Ok(result);
            }
            return NotFound("對方目前沒有上架的書籍");
        }
        /// <summary>
        /// 回應他人對自己的徵求，修改徵求的資訊
        /// </summary>
        /// <param name="seekId">徵求id</param>
        /// <param name="respondSeek">要修改的資料</param>
        /// <returns>回傳Status Code，200成功回應，404失敗</returns>
        // PUT api/<SeekController>/5
        [HttpPut("otherpeople/selectedbook/{seekId}")]
        public ActionResult Put(int seekId, [FromBody] RespondParameter respondSeek)
        {
            var result = SeekService.RespondSeek(seekId, respondSeek);
            if (result > 0)
            {
                return Ok("已回應對方徵求");
            }
            return NotFound("回應失敗");
        }
        /// <summary>
        /// 回絕他人對自己的徵求，修改徵求的資訊為取消
        /// </summary>
        /// <param name="seekId">徵求id</param>
        /// <returns>回傳Status Code，200回絕成功，404失敗</returns>
        [HttpPut("otherpeople/refusal/{seekId}/{userId}")]
        public ActionResult Put(int seekId,short userId)
        {
            var result = SeekService.RefusalSeek(seekId, userId);
            if (result > 0)
            {
                return Ok("已回絕徵求");
            }
            return NotFound("回應失敗");
        }
        /// <summary>
        /// 找出所有雙方選好書已經要進入寄送，但還未完成的紀錄
        /// </summary>
        /// <param name="userId">徵求id</param>
        /// <returns>回傳Status Code，200成功並附上媒合紀錄，404為沒找到</returns>
        // GET api/<SeekController>/5
        [HttpGet("match/{userId}")]
        public ActionResult GetMatch(short userId)
        {
            var result = SeekService.GetMatches(userId,Global.SeekStatus.Match);
            if (result.Count()>0) 
            {
                return Ok(result);
            }
            return NotFound("沒有任何媒合紀錄");
        }
        /// <summary>
        /// 當使用者寄出要交易的書籍時，將是否寄出改為true
        /// </summary>
        /// <param name="seekId">邀約id</param>
        /// <param name="userId">使用者id</param>
        /// <returns>回傳Status Code，200為操作成功，404為失敗</returns>
        // PUT api/<SeekController>/5
        [HttpPut("match/consignment/{seekId}")]
        public ActionResult PutForSend(int seekId, [FromBody] short userId)
        {
            var result = SeekService.BookSend(seekId, userId);
            if (result > 0)
            {
                return Ok("書籍已寄出");
            }
            return NotFound("操作失敗");
        }
        /// <summary>
        /// 當使用者收到交易的書籍時，將是否收到改為true
        /// </summary>
        /// <param name="seekId">邀約id</param>
        /// <param name="userId">使用者id</param>
        /// <returns>回傳Status Code，200為操作成功，404為失敗</returns>
        // PUT api/<SeekController>/5
        [HttpPut("match/receipt/{seekId}")]
        public ActionResult PutForReceive(int seekId, [FromBody] short userId)
        {
            var result = SeekService.BookReceive(seekId, userId);
            if (result > 0)
            {
                return Ok("書籍已收到");
            }
            return NotFound("操作失敗");
        }
        /// <summary>
        /// 取得是否這本書的攤主曾經對這位使用者發出過邀約
        /// </summary>
        /// <param name="userId">使用者Id</param>
        /// <param name="stallUserId">攤主Id</param>
        /// <returns>回傳Status Code+如果有回傳邀約的id和交易方式，沒有是null</returns>
        [HttpGet("book")]
        public ActionResult GetSeekExist(short userId, short stallUserId)
        {
            return Ok(SeekService.SeekExist(userId, stallUserId));
        }
        /// <summary>
        /// 找出這位使用者所有完成邀約的歷史紀錄
        /// </summary>
        /// <param name="userId">使用者ID</param>
        /// <returns>回傳Status Code，200成功並附上歷史紀錄，404為沒找到</returns>
        [HttpGet("history/{userId}")]
        public ActionResult GetHistory(short userId)
        {
            var result = SeekService.GetMatches(userId, Global.SeekStatus.Complete);
            if (result.Count() > 0)
            {
                return Ok(result);
            }
            return NotFound("沒有任何歷史紀錄");
        }
        /// <summary>
        /// 使用者給予對方在這筆邀約的評價
        /// </summary>
        /// <param name="seekId">邀約Id</param>
        /// <param name="evaluation">使用者Id+評價(1~5)</param>
        /// <returns>回傳Status Code</returns>
        [HttpPut("match/evaluation/{seekId}")]
        public ActionResult PutEvaluation(int seekId, [FromBody] EvaluationParameter evaluation)
        {
            var result = SeekService.SetEvaluation(seekId, evaluation);
            if (result > 0)
            {
                return Ok("已評價完畢");
            }
            return NotFound("評價失敗");
        }
    }
}
