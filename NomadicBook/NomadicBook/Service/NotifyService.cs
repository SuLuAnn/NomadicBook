using NomadicBook.Dto;
using NomadicBook.Models.db;
using NomadicBook.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static NomadicBook.Utils.Global;

namespace NomadicBook.Service
{
    public class NotifyService : INotifyService
    {
        private readonly NomadicBookContext NomadicBookContext;
        public NotifyService(NomadicBookContext nomadicBookContext)
        {
            NomadicBookContext = nomadicBookContext;
        }
        /// <summary>
        /// 取得該使用者的所有通知，在取得後所有通知都要變成已點擊
        /// </summary>
        /// <param name="userId">使用者ID</param>
        /// <returns>所有通知</returns>
        public List<NotifyDto> GetNotify(short userId)
        {
            var notifyList = NomadicBookContext.Notifies.Where(notify => notify.UserId == userId);
            var notifies = notifyList.OrderByDescending(notify => notify.NotifyId).Select(notify => new NotifyDto
            {
                Notify = notify.Notify1,
                NotifyDate = ChangeTime(notify.NotifyDate),
                Clicked = notify.Clicked
            }).ToList();
            notifyList.ToList().ForEach(notify => notify.Clicked = true);
            NomadicBookContext.SaveChanges();
            return notifies;
        }
        /// <summary>
        /// 取得該使用者的所有未讀通知數量
        /// </summary>
        /// <param name="userId">使用者ID</param>
        /// <returns>未讀通知數量</returns>
        public int GetNotifyNum(short userId)
        {
            return NomadicBookContext.Notifies.Where(notify => notify.UserId == userId && !notify.Clicked).Count();
        }
        /// <summary>
        /// 寄出新通知到資料庫
        /// </summary>
        /// <param name="userId">要被通知的使用者id</param>
        /// <param name="notify">通知內容</param>
        public void SendNotify(short userId, string notify)
        {
            NomadicBookContext.Notifies.Add(new Notify
            {
                UserId = userId,
                NotifyDate = DateTime.UtcNow.AddHours(08),
                Notify1=notify,
                Clicked=false
            });
            NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 依書本id取得書名
        /// </summary>
        /// <param name="bookId">書本id</param>
        /// <returns>書名</returns>
        public string GetBookName(int bookId)
        {
            if (bookId == 0)
            {
                return null;
            }
            return NomadicBookContext.Books.SingleOrDefault(book => book.BookId == bookId).BookName; 
        }
        /// <summary>
        /// 依使用者id取得使用者email
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <returns>email</returns>
        public string GetEmail(short userId)
        {
            if (userId == 0)
            {
                return null;
            }
            return NomadicBookContext.UserDatas.SingleOrDefault(user => user.UserId == userId).Email;
        }
        /// <summary>
        /// 當使用發起、回應邀約、寄出、收到書籍時，將訊息藉由email寄給邀約另一方，並將訊息紀錄至資料庫
        /// </summary>
        /// <param name="bookId">書本id</param>
        /// <param name="userId">邀約另一方的使用者id</param>
        /// <param name="num">當前要選擇的訊息代號</param>
        public void SendMessager(int bookId,short userId,MessengerNum num)
        {
            string messenger = string.Empty;
            switch (num)
            {
                case MessengerNum.Request:
                    messenger = $@"有人對您的 {GetBookName(bookId)} 提出交換申請。";
                    break;
                case MessengerNum.Match:
                    messenger = $@"對方已同意您對 {GetBookName(bookId)} 的邀約，請依照交換方式交換書籍，細節詳情請到交換媒合確認。";
                    break;
                case MessengerNum.OthersCancile:
                    messenger = $@"您對 {GetBookName(bookId)} 的交換申請已被對方拒絕。";
                    break;
                case MessengerNum.SelfCancile:
                    messenger = $@"對方已取消對 {GetBookName(bookId)} 的交換申請。";
                    break;
                case MessengerNum.Send:
                    messenger = $@"對方已寄出 {GetBookName(bookId)} 。";
                    break;
                case MessengerNum.Receive:
                    messenger = $@"對方已收到 {GetBookName(bookId)} 。";
                    break;
                case MessengerNum.LeaveMessage:
                    messenger = $@"您收到一則來自 {GetBookName(bookId)} 攤主的留言。";
                    break;
            }
            SendNotify(userId, messenger);//將對對方發的通知紀錄到資料庫
            Mail.SendMail(GetEmail(userId), "來自遊牧書籍的通知", messenger);//發email給對方
        }
        /// <summary>
        /// 取得這則邀約對方的使用者id及對方書的id
        /// </summary>
        /// <param name="seekId">邀約id</param>
        /// <param name="userId">自己的id</param>
        /// <returns>對方的使用者id及對方書的id</returns>
        public (int,short) GetOtherSideBook(int seekId, short userId)
        {
            var seek = NomadicBookContext.SeekRecords.SingleOrDefault(seek => seek.SeekId == seekId);
            if (seek != null)
            {
                if (seek.SeekUserId == userId)
                {
                    return ((int)seek.SeekedBookId, seek.SeekedUserId);
                }
                if (seek.SeekedUserId == userId)
                {
                    return (seek.SeekBookId, seek.SeekUserId);
                }
            }
            return (0,0);
        }

    }
}
