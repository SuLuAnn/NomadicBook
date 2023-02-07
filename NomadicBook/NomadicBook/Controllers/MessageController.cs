using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NomadicBook.Parameters;
using NomadicBook.Service;
using NomadicBook.Utils;
using static NomadicBook.Utils.Global;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NomadicBook.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService MessageService;
        private readonly INotifyService NotifyService;
        public MessageController(IMessageService messageService, INotifyService notifyService)
        {
            MessageService = messageService;
            NotifyService = notifyService;
        }
        /// <summary>
        /// 取得該邀約的所有留言
        /// </summary>
        /// <param name="seekId">邀約id</param>
        /// <returns>Status Code+所有留言</returns>
        // GET api/<MessageController>/5
        [HttpGet("{seekId}/{userId}")]
        public ActionResult Get(int seekId,short userId)
        {
         return Ok(MessageService.GetMessages(seekId, userId));
        }
        /// <summary>
        /// 新增一則留言
        /// </summary>
        /// <param name="message">留言內容</param>
        /// <returns>Status Code</returns>
        // POST api/<MessageController>
        [HttpPost]
        public ActionResult Post([FromBody] MessageParameter message)
        {
            string prompt = Global.IsCorrectLength(message.Message, StringLimit.Message);
            if (prompt != null)
            {
                return NotFound(prompt);
            }
            int result= MessageService.AddMessage(message);
            if (result > 0)
            {
                (int bookId, short userId) data = NotifyService.GetOtherSideBook(message.SeekId, message.UserId);
                NotifyService.SendMessager(data.bookId,data.userId,MessengerNum.LeaveMessage);
                return Ok("留言成功");
            }
            return NotFound("留言失敗");
        }
        /// <summary>
        /// 修改某則留言內容
        /// </summary>
        /// <param name="messageId">留言id</param>
        /// <param name="messageWord">新留言內容</param>
        /// <returns>Status Code</returns>
        // PUT api/<MessageController>/5
        [HttpPut("{messageId}")]
        public ActionResult Put(int messageId, [FromBody] string messageWord)
        {
            string prompt = Global.IsCorrectLength(messageWord, StringLimit.Message);
            if (prompt != null)
            {
                return NotFound(prompt);
            }
            int result = MessageService.SetMessage(messageId, messageWord);
            if (result > 0)
            {
                return Ok("留言已修改");
            }
            return NotFound("修改失敗");
        }
        /// <summary>
        /// 刪除一筆留言
        /// </summary>
        /// <param name="messageId">留言id</param>
        /// <returns>Status Code</returns>
        // DELETE api/<MessageController>/5
        [HttpDelete("{messageId}")]
        public ActionResult Delete(int messageId)
        {
            int result = MessageService.DeleteMessage(messageId);
            if (result > 0)
            {
                return Ok("留言已刪除");
            }
            return NotFound("刪除失敗");
        }
    }
}
