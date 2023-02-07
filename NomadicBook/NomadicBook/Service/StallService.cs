using NomadicBook.Dto;
using NomadicBook.Models.db;
using NomadicBook.Parameters;
using NomadicBook.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static NomadicBook.Utils.Global;

namespace NomadicBook.Service
{
    public class StallService: IStallService
    {
        private readonly NomadicBookContext NomadicBookContext;
        /// <summary>
        /// 注入資料庫
        /// </summary>
        /// <param name="nomadicBookContext">資料庫</param>
        public StallService(NomadicBookContext nomadicBookContext)
        {
            NomadicBookContext = nomadicBookContext;
        }
        /// <summary>
        ///取得該使用者攤位頁的所有書本資料 ( 含上下架 )
        /// </summary>
        /// <param name="userId">使用者ID</param>
        /// <returns>回傳使用者書本資料及徵求這本書的人數</returns>
        public List<StallBookDto> GetStallBooks(short userId) 
        {
            var list = NomadicBookContext.Books.Where(book => book.UserId == userId && book.BookExist).Select(book => new StallBookDto
            {
                BookId = book.BookId,
                BookName = book.BookName,
                Author = book.Author,
                Condition =ReplaceNull( book.Condition),
                ConditionNum = book.ConditionNum,
                BookStatus = book.BookStatus,
                BookPhoto = NomadicBookContext.BookPhotoes.FirstOrDefault(photo => book.BookId == photo.BookId).BookPhoto1,
                SeekNum = NomadicBookContext.SeekRecords.Where(record => record.SeekStatus == (byte)SeekStatus.Request&& record.SeekBookId == book.BookId).Count()
            }).OrderByDescending(book=>book.BookId).ToList();
            return list;
        }
        /// <summary>
        /// 使用者重新上架書籍
        /// </summary>
        /// <param name="bookId">書本id</param>
        /// <returns>被修改的資料筆數</returns>
        public int PutOnBook(int bookId)
        {
            var book = NomadicBookContext.Books.SingleOrDefault(book => book.BookId == bookId);
            if (book != null)
            {
                book.BookStatus = true;
            }
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 使用者下架書籍
        /// </summary>
        /// <param name="bookId">書本id</param>
        /// <returns>被修改的資料筆數</returns>
        public int PutOffBook(int bookId)
        {
            var book = NomadicBookContext.Books.SingleOrDefault(book => book.BookId == bookId);
            if (book != null)
            {
                book.BookStatus = false;
            }
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 更新書本訊息
        /// </summary>
        /// <param name="bookId">書本id</param>
        /// <param name="book">要更新的資訊</param>
        /// <returns>被修改的筆數</returns>
        public int UpdateBook(int bookId, BookUpdateParameter book)
        {
            var formerBook = NomadicBookContext.Books.SingleOrDefault(book => book.BookId == bookId && book.BookExist);
            if (formerBook == null)
            {
                return 0;
            }
            if (!formerBook.Experience.Equals(book.Experience)&&book.Experience!=null)
            {
                formerBook.ExperienceDay = DateTime.UtcNow.AddHours(08);
            }
            formerBook.PublishDate = ChangeTime(book.PublishDate);
            formerBook.Isbn = book.Isbn;
            formerBook.BookName = book.BookName;
            formerBook.Author = book.Author;
            formerBook.PublishingHouse = book.PublishingHouse;
            formerBook.CategoryId = book.CategoryId;
            formerBook.BookLong = book.BookLong;
            formerBook.BookWidth = book.BookWidth;
            formerBook.BookHigh = book.BookHigh;
            formerBook.Experience = book.Experience;
            formerBook.Introduction = book.Introduction;
            formerBook.Condition = book.Condition;
            formerBook.ConditionNum = book.ConditionNum;
            formerBook.StoreAddress = book.StoreAddress;
            formerBook.StoreName = book.StoreName;
            formerBook.MailBoxAddress = book.MailBoxAddress;
            formerBook.MailBoxName = book.MailBoxName;
            formerBook.HomeAddress = book.HomeAddress;
            formerBook.FaceTradeCity = book.FaceTradeCity;
            formerBook.FaceTradeArea = book.FaceTradeArea;
            formerBook.FaceTradeRoad = book.FaceTradeRoad;
            formerBook.FaceTradePath = book.FaceTradePath;
            formerBook.FaceTradeDetail = book.FaceTradeDetail;
            formerBook.TrueName = book.TrueName;
            formerBook.CellphoneNumber = book.CellphoneNumber;
            return NomadicBookContext.SaveChanges();
        }

    }
}
