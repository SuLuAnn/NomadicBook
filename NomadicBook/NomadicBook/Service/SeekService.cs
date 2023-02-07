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
    public class SeekService : ISeekService
    {
        private readonly NomadicBookContext NomadicBookContext;
        private readonly INotifyService NotifyService;
        /// <summary>
        /// 住入資料庫
        /// </summary>
        /// <param name="nomadicBookContext">資料庫</param>
        public SeekService(NomadicBookContext nomadicBookContext, INotifyService notifyService)
        {
            NomadicBookContext = nomadicBookContext;
            NotifyService = notifyService;
        }
        /// <summary>
        /// 新增一筆書本徵求
        /// </summary>
        /// <param name="newSeek">徵求資料</param>
        /// <returns>成功新增的筆數</returns>
        public int AddNewSeek(NewSeekParameter newSeek)
        {
            var book = NomadicBookContext.Books.Where(book => book.BookId == newSeek.SeekBookId && book.BookStatus && book.BookExist).FirstOrDefault();
            if (book == null)
            {
                return 0;
            }
            NomadicBookContext.SeekRecords.Add(new SeekRecord
            {
                SeekBookId = newSeek.SeekBookId,
                SeekUserId = newSeek.SeekUserId,
                TradeMode = newSeek.TradeMode,
                SeekToAddress = newSeek.SeekToAddress,
                SeekToName = newSeek.SeekToName,
                SeekedUserId = newSeek.SeekedUserId,
                SeekDate = DateTime.UtcNow.AddHours(08),
                SeekSend = false,
                SeekReceive = false,
                SeekStatus = (byte)SeekStatus.Request,
                SeekedName = book.TrueName,
                SeekedCellphone = book.CellphoneNumber,
                SeekedSend = false,
                SeekedReceive = false
            });
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 取得所有尚未被回應的我的徵求紀錄
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <returns>所有尚未被回應的我的徵求紀錄</returns>
        public List<MySeekDto> GetMySeek(short userId)
        {
            var mySeekList = NomadicBookContext.SeekRecords.Where(record => record.SeekUserId == userId && record.SeekStatus == (byte)SeekStatus.Request)
                .Join(NomadicBookContext.Books, record => record.SeekBookId, book => book.BookId, (record, book) => new MySeekDto
                {
                    SeekBookId = record.SeekBookId,
                    SeekBookName = book.BookName,
                    SeekId = record.SeekId,
                    SeekBookPhoto = NomadicBookContext.BookPhotoes.Where(photo => record.SeekBookId == photo.BookId).FirstOrDefault().BookPhoto1,
                    Author = book.Author,
                    Condition = ReplaceNull(book.Condition),
                    ConditionNum = book.ConditionNum,
                    SeekStatus = book.BookExist && book.BookStatus,
                    TradeMode = record.TradeMode,
                    SeekDate = ChangeTime(record.SeekDate)
                }).OrderByDescending(record => record.SeekId).ToList();
            return mySeekList;
        }
        /// <summary>
        /// 別人對使用者的徵求，並且使用者尚未回應的邀請
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <returns>所有尚未被回應的別人對使用者的徵求紀錄</returns>
        public List<OtherSeekDto> GetOtherSeek(short userId)
        {
            var otherSeekList = NomadicBookContext.SeekRecords.Where(record => record.SeekedUserId == userId && record.SeekStatus == (byte)SeekStatus.Request)
               .Join(NomadicBookContext.Books, record => record.SeekBookId, book => book.BookId, (record, book) => new OtherSeekDto
               {
                   SeekBookId = record.SeekBookId,
                   SeekBookName = book.BookName,
                   SeekId = record.SeekId,
                   SeekUserId = record.SeekUserId,
                   TradeMode = record.TradeMode,
                   SeekDate = ChangeTime(record.SeekDate),
                   SeekBookPhoto = NomadicBookContext.BookPhotoes.Where(photo => record.SeekBookId == photo.BookId).FirstOrDefault().BookPhoto1,
                   Author = book.Author,
                   Condition = ReplaceNull(book.Condition),
                   ConditionNum = book.ConditionNum,
                   SeekStatus = book.BookExist && book.BookStatus
               }).OrderByDescending(record => record.SeekId).ToList();
            return otherSeekList;
        }
        /// <summary>
        /// 在回應別人對自身的徵求時，要選擇一本對方所擁有的書做交換，並且這本書要擁有與對方的徵求所選擇相同的交易方式
        /// </summary>
        /// <param name="userId">對方的使用者id</param>
        /// <param name="tradeMode">交易方式編號</param>
        /// <returns>回傳所有對方所上架並且交易方式相同的書籍</returns>
        public List<ListProductDto> GetOtherBook(short userId, byte tradeMode)
        {
            var otherBookList = NomadicBookContext.Books.Where(book => book.UserId == userId && book.BookStatus && book.BookExist);
            switch ((TradeMode)tradeMode)
            {
                case TradeMode.StoreToStore:
                    otherBookList = otherBookList.Where(book => book.StoreAddress != null);
                    break;
                case TradeMode.Delivery:
                    otherBookList = otherBookList.Where(book => book.HomeAddress != null);
                    break;
                case TradeMode.MailBox:
                    otherBookList = otherBookList.Where(book => book.MailBoxAddress != null);
                    break;
                case TradeMode.FaceTrade:
                    otherBookList = otherBookList.Where(book => book.FaceTradeCity != null);
                    break;
            }
            var listResult = otherBookList.Select(book => new ListProductDto
            {
                BookId = book.BookId,
                BookName = book.BookName,
                Author = book.Author,
                BookPhoto = NomadicBookContext.BookPhotoes.FirstOrDefault(photo => book.BookId == photo.BookId).BookPhoto1,
                Condition = ReplaceNull(book.Condition),
                ConditionNum = book.ConditionNum
            }).OrderByDescending(book => book.BookId).ToList();
            return listResult;
        }
        /// <summary>
        /// 回應他人對自己的徵求，修改徵求的資訊
        /// </summary>
        /// <param name="seekId">徵求id</param>
        /// <param name="respondSeek">要修改的資料</param>
        /// <returns>回傳資料庫修改的筆數</returns>
        public int RespondSeek(int seekId, RespondParameter respondSeek)
        {
            var book = NomadicBookContext.Books.SingleOrDefault(book => book.BookId == respondSeek.BookId && book.BookStatus && book.BookExist);
            var seek = NomadicBookContext.SeekRecords.SingleOrDefault(record => record.SeekId == seekId && record.SeekStatus == (byte)SeekStatus.Request);
            var seekBook = NomadicBookContext.Books.SingleOrDefault(book => book.BookId == seek.SeekBookId && book.BookStatus && book.BookExist);
            if (seek != null && book != null && seekBook != null)
            {
                NotifyService.SendMessager(seek.SeekBookId, seek.SeekUserId, MessengerNum.Match);
                book.BookExist = false;
                seekBook.BookExist = false;
                seek.SeekName = book.TrueName;
                seek.SeekCellphone = book.CellphoneNumber;
                seek.SeekedDate = DateTime.UtcNow.AddHours(08);
                seek.SeekedBookId = respondSeek.BookId;
                seek.SeekStatus = (byte)SeekStatus.Match;
                byte tradeMode = respondSeek.TradeMode;
                switch ((TradeMode)tradeMode)
                {
                    case TradeMode.StoreToStore:
                        seek.SeekedToAddress = book.StoreAddress;
                        seek.SeekedToName = book.StoreName;
                        break;
                    case TradeMode.Delivery:
                        seek.SeekedToAddress = book.HomeAddress;
                        break;
                    case TradeMode.MailBox:
                        seek.SeekedToAddress = book.MailBoxAddress;
                        seek.SeekedToName = book.MailBoxName;
                        break;
                    case TradeMode.FaceTrade:
                        seek.SeekedToAddress = seek.SeekToAddress;
                        break;
                }
            }
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 回絕他人對自己的徵求，修改徵求的資訊為取消
        /// </summary>
        /// <param name="seekId">徵求id</param>
        /// <returns>回傳資料庫修改的筆數</returns>
        public int RefusalSeek(int seekId, short userId)
        {
            var seek = NomadicBookContext.SeekRecords.SingleOrDefault(record => record.SeekId == seekId);
            if (seek == null)
            {
                return 0;
            }
            if (seek.SeekUserId == userId) 
            {
                NotifyService.SendMessager(seek.SeekBookId, seek.SeekedUserId, MessengerNum.SelfCancile);
                seek.SeekStatus = (byte)SeekStatus.Cancile;
            }
            if (seek.SeekedUserId == userId)
            {
                NotifyService.SendMessager(seek.SeekBookId, seek.SeekUserId, MessengerNum.OthersCancile);
                seek.SeekStatus = (byte)SeekStatus.Cancile;
            }
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 找出所有雙方選好書的紀錄，狀態有可能是歷史紀錄也有可能是未完成的紀錄
        /// </summary>
        /// <param name="userId">徵求id</param>
        /// /// <param name=" status">紀錄的狀態</param>
        /// <returns>回傳媒合紀錄集合</returns>
        public List<MatchDto> GetMatches(short userId, SeekStatus status)
        {
            NomadicBookContext.SeekRecords.Where(record => record.SeekStatus == (byte)SeekStatus.Match && record.SeekReceive == true && record.SeekReceive == record.SeekedReceive).ToList().ForEach(record => record.SeekStatus = (byte)SeekStatus.Complete);
            NomadicBookContext.SaveChanges();
            var books = NomadicBookContext.Books.Where(book => book.BookStatus == true);
            var photo = NomadicBookContext.BookPhotoes.Select(photo => photo);
            var matchList = NomadicBookContext.SeekRecords.Where(record => record.SeekStatus == (byte)status && (record.SeekUserId == userId || record.SeekedUserId == userId));
            var list = matchList.OrderByDescending(seek => seek.SeekedDate).Select(record => new MatchDto
            {
                SeekId = record.SeekId,
                TradeMode = record.TradeMode,
                Seek = new MatchDetailDto
                {
                    SeekUserId = record.SeekUserId,
                    SeekBookId = (int)record.SeekedBookId,
                    SeekAuthor = books.SingleOrDefault(book => book.BookId == record.SeekedBookId).Author,
                    SeekBookName = books.SingleOrDefault(book => book.BookId == record.SeekedBookId).BookName,
                    SeekBookPhoto = photo.FirstOrDefault(photo => photo.BookId == record.SeekedBookId).BookPhoto1,
                    SeekCondition = ReplaceNull(books.SingleOrDefault(book => book.BookId == record.SeekedBookId).Condition),
                    SeekConditionNum = books.SingleOrDefault(book => book.BookId == record.SeekedBookId).ConditionNum,
                    SeekUserName = NomadicBookContext.UserDatas.SingleOrDefault(user => user.UserId == record.SeekUserId).NickName,
                    Evaluation = record.SeekedEvaluation,
                    TradeNum = NomadicBookContext.UserDatas.SingleOrDefault(user => user.UserId == record.SeekUserId).TradeNum,
                    SeekName = record.SeekName,
                    SeekCellphone = record.SeekCellphone,
                    SeekDate = ChangeTime(record.SeekDate),
                    SeekToAddress = record.SeekedToAddress,
                    SeekToName = ReplaceNull(record.SeekedToName),
                    SeekSend = record.SeekSend,
                    SeekReceive = record.SeekReceive
                },
                Seeked = new MatchDetailDto
                {
                    SeekUserId = record.SeekedUserId,
                    SeekBookId = record.SeekBookId,
                    SeekAuthor = books.SingleOrDefault(book => book.BookId == record.SeekBookId).Author,
                    SeekBookName = books.SingleOrDefault(book => book.BookId == record.SeekBookId).BookName,
                    SeekBookPhoto = photo.FirstOrDefault(photo => photo.BookId == record.SeekBookId).BookPhoto1,
                    SeekCondition = ReplaceNull(books.SingleOrDefault(book => book.BookId == record.SeekBookId).Condition),
                    SeekConditionNum = books.SingleOrDefault(book => book.BookId == record.SeekBookId).ConditionNum,
                    SeekUserName = NomadicBookContext.UserDatas.SingleOrDefault(user => user.UserId == record.SeekedUserId).NickName,
                    Evaluation = record.SeekEvaluation,
                    TradeNum = NomadicBookContext.UserDatas.SingleOrDefault(user => user.UserId == record.SeekedUserId).TradeNum,
                    SeekName = record.SeekedName,
                    SeekCellphone = record.SeekedCellphone,
                    SeekDate = ChangeTime(record.SeekedDate),
                    SeekToAddress = record.SeekToAddress,
                    SeekToName = ReplaceNull(record.SeekToName),
                    SeekSend = record.SeekedSend,
                    SeekReceive = record.SeekedReceive
                }
            }).ToList();
            foreach (var match in list)
            {
                if (userId != match.Seek.SeekUserId)
                {
                    MatchDetailDto tmp = match.Seek;
                    match.Seek = match.Seeked;
                    match.Seeked = tmp;
                }
            }
            return list;
        }
        /// <summary>
        /// 當使用者寄出要交易的書籍時，將是否寄出改為true
        /// </summary>
        /// <param name="seekId">邀約id</param>
        /// <param name="userId">使用者id</param>
        /// <returns>回傳資料庫修改的筆數</returns>
        public int BookSend(int seekId, short userId)
        {
            var record = NomadicBookContext.SeekRecords.SingleOrDefault(record => record.SeekId == seekId && (record.SeekStatus == (byte)SeekStatus.Match || record.SeekStatus == (byte)SeekStatus.Complete));
            if (record == null)
            {
                return 0;
            }
            if (userId == record.SeekUserId)
            {
                NotifyService.SendMessager((int)record.SeekedBookId, record.SeekedUserId, MessengerNum.Send);
                record.SeekSend = true;
            }
            if (userId == record.SeekedUserId)
            {
                NotifyService.SendMessager((int)record.SeekBookId, record.SeekUserId, MessengerNum.Send);
                record.SeekedSend = true;
            }
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 當使用者收到交易的書籍時，將是否收到改為true
        /// </summary>
        /// <param name="seekId">邀約id</param>
        /// <param name="userId">使用者id</param>
        /// <returns>回傳資料庫修改的筆數</returns>
        public int BookReceive(int seekId, short userId)
        {
            var record = NomadicBookContext.SeekRecords.SingleOrDefault(record => record.SeekId == seekId && record.SeekStatus == (byte)SeekStatus.Match);
            if (record == null)
            {
                return 0;
            }
            if (userId == record.SeekUserId)
            {
                NotifyService.SendMessager((int)record.SeekBookId, record.SeekedUserId, MessengerNum.Receive);
                record.SeekReceive = true;
                SetUserEvaluation(record.SeekedUserId, (double)record.SeekEvaluation);
            }
            if (userId == record.SeekedUserId)
            {
                NotifyService.SendMessager((int)record.SeekedBookId, record.SeekUserId, MessengerNum.Receive);
                record.SeekedReceive = true;
                SetUserEvaluation(record.SeekUserId, (double)record.SeekedEvaluation);
            }
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 確認使用者是否有被這書的攤主邀約過
        /// </summary>
        /// <param name="userId">使用者Id</param>
        /// <param name="stallUserId">攤主Id</param>
        /// <returns>如果有回傳邀約的id和交易方式，沒有是null</returns>
        public SeekExistDto SeekExist(short userId, short stallUserId)
        {
            var seek = NomadicBookContext.SeekRecords.FirstOrDefault(seek => userId == seek.SeekedUserId && stallUserId == seek.SeekUserId && seek.SeekStatus == (byte)SeekStatus.Request);
            if (seek == null)
            {
                return null;
            }
            return new SeekExistDto
            {
                SeekId = seek.SeekId,
                TradeMode = seek.TradeMode
            };
        }
        /// <summary>
        /// 使用者給予對方在這筆邀約的評價
        /// </summary>
        /// <param name="seekId">邀約Id</param>
        /// <param name="evaluation">使用者Id+評價(1~5)</param>
        /// <returns>回傳資料庫修改的筆數</returns>
        public int SetEvaluation(int seekId, EvaluationParameter evaluation)
        {
            var seek = NomadicBookContext.SeekRecords.SingleOrDefault(seek => seek.SeekId == seekId);
            if (seek == null || evaluation.Evaluation < 0 || evaluation.Evaluation > 5)
            {
                return 0;
            }
            if (seek.SeekUserId == evaluation.UserId)
            {
                seek.SeekEvaluation = evaluation.Evaluation;
            }
            if (seek.SeekedUserId == evaluation.UserId)
            {
                seek.SeekedEvaluation = evaluation.Evaluation;
            }
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 當雙方都完成邀約，將評價放入使用者本身的會員資料
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <param name="evaluation">這次邀約被給的評價</param>
        public void SetUserEvaluation(short userId, double evaluation)
        {
            var user = NomadicBookContext.UserDatas.SingleOrDefault(user => user.UserId == userId);
            if (user != null)
            {
                user.TotalEvaluation += evaluation;
                user.TradeNum++;
            }
        }
    }
}
