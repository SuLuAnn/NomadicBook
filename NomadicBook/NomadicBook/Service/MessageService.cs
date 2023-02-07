using NomadicBook.Dto;
using NomadicBook.Models.db;
using NomadicBook.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using static NomadicBook.Utils.Global;

namespace NomadicBook.Service
{
    public class MessageService: IMessageService
    {
        private readonly NomadicBookContext NomadicBookContext;
        public MessageService(NomadicBookContext nomadicBookContext)
        {
            NomadicBookContext = nomadicBookContext;
        }
        /// <summary>
        /// 新增一則留言
        /// </summary>
        /// <param name="message">留言內容</param>
        /// <returns>被修改的資料數量</returns>
        public int AddMessage(MessageParameter message)
        {
            NomadicBookContext.RoomMessages.Add(new RoomMessage
            {
                SeekId=message.SeekId,
                UserId=message.UserId,
                MessageTime= DateTime.UtcNow.AddHours(08),
                Message=message.Message
            });
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 刪除一筆留言
        /// </summary>
        /// <param name="messageId">留言id</param>
        /// <returns>被修改的資料數量</returns>
        public int DeleteMessage(int messageId)
        {
            var message = NomadicBookContext.RoomMessages.SingleOrDefault(message=>message.MessageId==messageId);
            if (message != null)
            {
                NomadicBookContext.RoomMessages.Remove(message);
            }
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 取得該邀約的所有留言
        /// </summary>
        /// <param name="seekId">邀約id</param>
        /// /// <param name="userId">使用者id</param>
        /// <returns>所有留言</returns>
        public List<MessageDto> GetMessages(int seekId, short userId)
        {
            var list = NomadicBookContext.RoomMessages.Where(message => message.SeekId == seekId).Join(NomadicBookContext.UserDatas, message=>message.UserId,user=>user.UserId, (message,user) => new MessageDto
            {
                MessageId=message.MessageId,
                UserName=user.NickName,
                UserPhoto=user.UserPhoto,
                MessageTime= ChangeHourTime(message.MessageTime),
                Message=message.Message,
                IsOwner=message.UserId==userId
            }).OrderByDescending(message=>message.MessageId).ToList();
            return list;
        }
        /// <summary>
        /// 修改某則留言內容
        /// </summary>
        /// <param name="messageId">留言id</param>
        /// <param name="messageWord">新留言內容</param>
        /// <returns>被修改的資料數量</returns>
        public int SetMessage(int messageId, string messageWord)
        {
            var message = NomadicBookContext.RoomMessages.SingleOrDefault(message => message.MessageId == messageId);
            if (message != null)
            {
                message.Message = messageWord;
            }
            return NomadicBookContext.SaveChanges();
        }
        
    }
}
