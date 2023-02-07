using NomadicBook.Dto;
using NomadicBook.Models.db;
using System.Collections.Generic;
using System.Linq;
using static NomadicBook.Utils.Global;

namespace NomadicBook.Service
{
    public class ListService: IListService
    {
        private readonly NomadicBookContext NomadicBookContext;
        public ListService(NomadicBookContext nomadicBookContext)
        {
            NomadicBookContext = nomadicBookContext;
        }
        /// <summary>
        /// 取得最新更新過心得的書籍資訊
        /// </summary>
        /// <param name="max">要取的最多筆數</param>
        /// <returns>書籍資訊</returns>
        public List<ExperienceListDto> GetExperienceList(int max)
        {
            var list = NomadicBookContext.Books.Where(book => book.Experience != null && book.BookStatus && book.BookExist)
                .OrderByDescending(book => book.ExperienceDay).ThenByDescending(book=> book.BookId)
                .Select(book => new ExperienceListDto
                {
                    BookId = book.BookId,
                    BookName = book.BookName,
                    UserId=book.UserId,
                    UserName= NomadicBookContext.UserDatas.SingleOrDefault(user=>user.UserId==book.UserId).NickName,
                    Author = book.Author,
                    BookPhoto = NomadicBookContext.BookPhotoes.FirstOrDefault(photo => photo.BookId == book.BookId).BookPhoto1,
                    ConditionNum=book.ConditionNum,
                    Condition= ReplaceNull(book.Condition),
                    Experience= ReplaceNull(book.Experience),
                    ExperienceDay = ChangeTime(book.ExperienceDay)
                }).AsEnumerable();
            if (max > 0)
            {
                list = list.Take(max);
            }
            return list.ToList();
        }
        /// <summary>
        /// 取得最新出版的書籍資訊
        /// </summary>
        /// <param name="max">要取的最多筆數</param>
        /// <returns>書籍資訊</returns>
        public List<ListProductDto> GetPublishDayList(int max)
        {
            var list = NomadicBookContext.Books.Where(book => book.BookStatus && book.BookExist).OrderByDescending(book => book.PublishDate)
               .Select(book => new ListProductDto
               {
                   BookId = book.BookId,
                   BookName = book.BookName,
                   Author = book.Author,
                   BookPhoto = NomadicBookContext.BookPhotoes.FirstOrDefault(photo => photo.BookId == book.BookId).BookPhoto1,
                   ConditionNum = book.ConditionNum,
                   Condition = ReplaceNull(book.Condition),
               }).AsEnumerable();
            if (max > 0)
            {
                list = list.Take(max);
            }
            return list.ToList();
        }
        /// <summary>
        /// 取得最新上架的書籍資訊
        /// </summary>
        /// <param name="max">要取的最多筆數</param>
        /// <returns>書籍資訊</returns>
        public List<ListProductDto> GetNewBook(int max) 
        {
            var list = NomadicBookContext.Books.Where(book => book.BookStatus && book.BookExist)
               .Select(book => new ListProductDto
               {
                   BookId = book.BookId,
                   BookName = book.BookName,
                   Author = book.Author,
                   BookPhoto = NomadicBookContext.BookPhotoes.FirstOrDefault(photo => photo.BookId == book.BookId).BookPhoto1,
                   ConditionNum = book.ConditionNum,
                   Condition = ReplaceNull(book.Condition)
               }).OrderByDescending(book => book.BookId).AsEnumerable();
            if (max > 0)
            {
                list = list.Take(max);
            }
            return list.ToList();
        }
    }
}
