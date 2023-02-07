using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class HomePageController : ControllerBase
    {
        private readonly IListService ListService;
        private readonly INotifyService NotifyService;
        public HomePageController(IListService listService, INotifyService notifyService)
        {
            ListService = listService;
            NotifyService = notifyService;
        }
        /// <summary>
        /// 取得該使用者的所有通知，在取得後所有通知都要變成已點擊
        /// </summary>
        /// <param name="userId">使用者ID</param>
        /// <returns>Status Code+所有通知</returns>
        // GET api/<HomePageController>/5
        [Authorize]
        [HttpGet("notify/{userId}")]
        public ActionResult GetNotify(short userId)
        {
            var notify = NotifyService.GetNotify(userId);
            if (notify.Count() > 0)
            {
                return Ok(notify);
            }
            return NotFound("當前無任何通知");
        }
        /// <summary>
        /// 取得該使用者的所有未讀通知數量
        /// </summary>
        /// <param name="userId">使用者ID</param>
        /// <returns>Status Code+未讀通知數量</returns>
        [Authorize]
        [HttpGet("notifynum/{userId}")]
        public ActionResult GetNotifyNum(short userId)
        {
            int num = NotifyService.GetNotifyNum(userId);
            return Ok(num);
        }

        /// <summary>
        /// 取得最新更新過心得的書籍資訊
        /// </summary>
        /// <param name="max">要取的最多筆數</param>
        /// <returns>Status Code+書籍資訊</returns>
        // GET api/<HomePageController>/5
        [HttpGet("experiencelist")]
        public ActionResult GetExperience(int max)
        {
            var experience = ListService.GetExperienceList(max);
            if (experience.Count() > 0)
            {
                return Ok(experience);
            }
            return NotFound("當前無任何心得");
        }

        /// <summary>
        /// 取得最新出版的書籍資訊
        /// </summary>
        /// <param name="max">要取的最多筆數</param>
        /// <returns>Status Code+書籍資訊</returns>
        // GET api/<HomePageController>/5
        [HttpGet("publishdaylist")]
        public ActionResult GetPublishDay(int max)
        {
            var publishList= ListService.GetPublishDayList(max);
            if (publishList.Count() > 0)
            {
                return Ok(publishList);
            }
            return NotFound("當前無任何上架書籍");
        }
        /// <summary>
        /// 取得最新上架的書籍資訊
        /// </summary>
        /// <param name="max">要取的最多筆數</param>
        /// <returns>Status Code+書籍資訊</returns>
        [HttpGet("newbooklist")]
        public ActionResult GetNewBook(int max)
        {
            var newList = ListService.GetNewBook(max);
            if (newList.Count() > 0)
            {
                return Ok(newList);
            }
            return NotFound("當前無任何上架書籍");
        }
    }
}
