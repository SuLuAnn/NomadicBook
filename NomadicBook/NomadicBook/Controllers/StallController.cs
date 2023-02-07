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
    public class StallController : ControllerBase
    {
        private readonly IStallService StallService;
        private readonly IPhotoService PhotoService;
        private readonly Photo Photo;
        public StallController(IStallService stallService, Photo photo, IPhotoService photoService)
        {
            StallService = stallService;
            Photo = photo;
            PhotoService = photoService;
        }
        /// <summary>
        ///取得該使用者攤位頁的所有書本資料 ( 含上下架 )
        /// </summary>
        /// <param name="userId">使用者ID</param>
        /// <returns>回傳Status Code+使用者書本資料及徵求這本書的人數</returns>
        // GET: api/<ValuesController>
        [AllowAnonymous]
        [HttpGet("{userId}")]
        public ActionResult Get(short userId)
        {
            var stallBooks = StallService.GetStallBooks(userId);
            if (stallBooks != null)
            {
                return Ok(stallBooks);
            }
            return NotFound("查無資料");
        }
        /// <summary>
        /// 使用者重新上架書籍
        /// </summary>
        /// <param name="bookId">書本id</param>
        /// <returns>Status Code</returns>
        // PUT api/<ValuesController>/5
        [HttpPut("bookon/{bookId}")]
        public ActionResult PutOn(int bookId)
        {
            if (StallService.PutOnBook(bookId) > 0)
            {
                return Ok("重新上架成功");
            }
            return NotFound("重新上架失敗");
        }
        /// <summary>
        /// 使用者下架書籍
        /// </summary>
        /// <param name="bookId">書本id</param>
        /// <returns>Status Code</returns>
        // PUT api/<ValuesController>/5
        [HttpPut("bookoff/{bookId}")]
        public ActionResult PutOff(int bookId)
        {
            if (StallService.PutOffBook(bookId) > 0)
            {
                return Ok("下架成功");
            }
            return NotFound("下架失敗");
        }
        /// <summary>
        /// 更新書本訊息
        /// </summary>
        /// <param name="bookId">書本id</param>
        /// <param name="book">要更新的資訊</param>
        /// <returns>Status Code</returns>
        // PUT api/<ValuesController>/5
        [HttpPut("bookupdate/{bookId}")]
        public async Task< ActionResult> Put(int bookId,[FromForm] BookUpdateParameter book )
        {
            List<string> prompts = new List<string>();
            prompts.Add(Global.IsCorrectLength(book.Isbn, StringLimit.ISBN));
            prompts.Add(Global.IsCorrectLength(book.BookName, StringLimit.BookName));
            prompts.Add(Global.IsCorrectLength(book.Author, StringLimit.Author));
            prompts.Add(Global.IsCorrectLength(book.PublishingHouse, StringLimit.PublishingHouse));
            prompts.Add(Global.IsCorrectLength(book.Experience, StringLimit.Experience));
            prompts.Add(Global.IsCorrectLength(book.Introduction, StringLimit.Introduction));
            prompts.Add(Global.IsCorrectLength(book.Condition, StringLimit.Condition));
            prompts.Add(Global.IsCorrectLength(book.HomeAddress, StringLimit.Address));
            prompts.Add(Global.IsCorrectLength(book.FaceTradePath, StringLimit.Path));
            prompts.Add(Global.IsCorrectLength(book.FaceTradeDetail, StringLimit.Detail));
            prompts.Add(Global.IsCorrectLength(book.TrueName, StringLimit.TrueName));
            prompts.Add(Global.IsCorrectLength(book.CellphoneNumber, StringLimit.CellphoneNumber));
            foreach (var prompt in prompts)
            {
                if (prompt != null)
                {
                    return NotFound(prompt);
                }
            }
            List<string> photoPaths = new List<string>();
            int i = 0;
            var photos = book.BookPhoto;
            int photoNum = 0;
            if (photos != null)
            {
                foreach (var photo in photos)
                {
                    string fileName = $"{ DateTime.UtcNow.AddHours(08).ToString("yyyyMMddHHmmss")}{bookId}{i++}";
                    photoPaths.Add(fileName);
                    await Photo.Upload(photo, fileName);
                }
                photoNum = PhotoService.UploadPhoto(bookId, photoPaths);
            }
            if (StallService.UpdateBook(bookId,book) > 0 || photoNum>0)
            {
                return Ok("修改成功");
            }
            return NotFound("修改失敗");
        }
        /// <summary>
        /// 刪除指定id圖片
        /// </summary>
        /// <param name="photoId">圖片id</param>
        /// <returns>Status Code</returns>
        // DELETE api/<ValuesController>/5
        [HttpDelete("{photoId}")]
        public ActionResult Delete(int photoId)
        {
            string photoName = PhotoService.DeletePhoto(photoId);
            if (photoName!=null)
            {
                Photo.Delete(photoName);
                return Ok("圖片已刪除");
            }
            return NotFound("刪除失敗");
        }
    }
}
