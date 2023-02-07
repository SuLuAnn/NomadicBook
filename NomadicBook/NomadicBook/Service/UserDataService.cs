using Microsoft.AspNetCore.Http;
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
    public class UserDataService : IUserDataService
    {
        private readonly NomadicBookContext NomadicBookContext;
        public UserDataService(NomadicBookContext nomadicBookContext)
        {
            NomadicBookContext = nomadicBookContext;
        }
        /// <summary>
        /// 回傳使用者基本資料
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <returns>基本資料</returns>
        public BasicUserDto GetBasicData(short userId)
        {
            return NomadicBookContext.UserDatas.Where(user => user.UserId == userId).Select(user =>new BasicUserDto{
                NickName=user.NickName,
                Email=user.Email,
                UserPhoto=ReplaceNull( user.UserPhoto),
                SelfIntroduction= ReplaceNull(user.SelfIntroduction),
                Evaluation= ReplaceUndefined(user.TotalEvaluation, user.TradeNum),
                TradeNum=user.TradeNum
            }).SingleOrDefault();
        }
        /// <summary>
        /// 取得使用者詳細資料
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <returns>使用者詳細資料</returns>
        public DetailUserDto GetDetailData(short userId)
        {
            return NomadicBookContext.UserDatas.Where(user => user.UserId == userId).Select(user => new DetailUserDto
            {
                NickName = user.NickName,
                Email = user.Email,
                UserPhoto = ReplaceNull(user.UserPhoto),
                SelfIntroduction = ReplaceNull(user.SelfIntroduction),
                StoreAddress =ReplaceNull(user.StoreAddress),
                StoreName=ReplaceNull(user.StoreName),
                MailBoxAddress=ReplaceNull(user.MailBoxAddress),
                MailBoxName=ReplaceNull(user.MailBoxName),
                HomeAddress= ReplaceNull(user.HomeAddress),
                FaceTradeCity= ReplaceNull(user.FaceTradeCity),
                FaceTradeArea=ReplaceNull(user.FaceTradeArea),
                FaceTradeRoad= ReplaceNull(user.FaceTradeRoad),
                FaceTradePath= ReplaceNull(user.FaceTradePath),
                FaceTradeDetail= ReplaceNull(user.FaceTradeDetail),
                TrueName= ReplaceNull(user.TrueName),
                CellphoneNumber= ReplaceNull(user.CellphoneNumber)
            }).SingleOrDefault();
        }
        /// <summary>
        /// 修改使用者詳細資料
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <param name="user">要更新的使用者資料</param>
        /// <returns>回傳被修改筆數，要新增的頭貼圖片檔名，要刪除的舊頭貼檔名</returns>
        public (int, string, string) Update(short userId, UserParameter user)
        {
            var userData = NomadicBookContext.UserDatas.SingleOrDefault(user => user.UserId == userId);
            string photoName = null;
            string oldName = null;
            if (userData != null)
            {
                userData.NickName = user.NickName;
                userData.SelfIntroduction = user.SelfIntroduction;
                userData.StoreAddress =user.StoreAddress;
                userData.StoreName = user.StoreName;
                userData.MailBoxAddress = user.MailBoxAddress;
                userData.MailBoxName = user.MailBoxName;
                userData.HomeAddress = user.HomeAddress;
                userData.FaceTradeCity = user.FaceTradeCity;
                userData.FaceTradeArea = user.FaceTradeArea;
                userData.FaceTradeRoad = user.FaceTradeRoad;
                userData.FaceTradePath = user.FaceTradePath;
                userData.FaceTradeDetail =user.FaceTradeDetail;
                userData.TrueName = user.TrueName;
                userData.CellphoneNumber = user.CellphoneNumber;
                oldName = userData.UserPhoto;
                if (user.UserPhoto != null)
                {
                    photoName = $"{DateTime.UtcNow.AddHours(08).ToString("yyyyMMddHHmmss")}{userId}";
                    userData.UserPhoto = photoName;
                }
            }
            return (NomadicBookContext.SaveChanges(),photoName, oldName);
        }
    }
}
