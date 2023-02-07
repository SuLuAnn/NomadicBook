using Microsoft.IdentityModel.Tokens;
using NomadicBook.Dto;
using NomadicBook.Models.db;
using NomadicBook.Parameters;
using NomadicBook.Utils;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using static NomadicBook.Utils.Global;

namespace NomadicBook.Service
{
    public class SignInService: ISignInService
    {
        private readonly NomadicBookContext NomadicBookContext;
        /// <summary>
        /// 注入資料庫
        /// </summary>
        /// <param name="nomadicBookContext">資料庫</param>
        public SignInService(NomadicBookContext nomadicBookContext) 
        {
            NomadicBookContext = nomadicBookContext;
        }
        /// <summary>
        /// email是否可以使用
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>可以與否的boolean值</returns>
        public bool CanUseEmail(string email)
        {
            if (email == null) 
            {
                return false;
            }
            return NomadicBookContext.UserDatas.Where(user=>user.Email.Equals(email)).Count()<1;
        }
        /// <summary>
        /// 暱稱是否可以使用
        /// </summary>
        /// <param name="nickname">暱稱</param>
        /// <returns>可以與否的boolean值</returns>
        public bool CanUseNickName(string nickname)
        {
            if (nickname == null)
            {
                return false;
            }
            return NomadicBookContext.UserDatas.Where(user => user.NickName.Equals(nickname)).Count() < 1;
        }
        /// <summary>
        /// 確認使用者傳入密碼、email是否存在，並回傳使用者登入資訊
        /// </summary>
        /// <param name="signInParameter">包含密碼、email的物件</param>
        /// <returns>包含使用者id、暱稱的物件</returns>
        public SignInDto GetSignInData(SignInParameter signInParameter)
        {
            string password = Security.Encryption(signInParameter.Password);
            string email = signInParameter.Email;
            var user = NomadicBookContext.UserDatas.SingleOrDefault(user => user.Password.Equals(password) && user.Email.Equals(email));
            if(user==null)
            {
                return null;
            }
            return new SignInDto
            {
                UserId = user.UserId,
                NickName = user.NickName,
                Authenticate = Authenticate(user)
            };
        }
        /// <summary>
        /// 將使用者傳入的密碼、email、暱稱作為註冊資料傳入資料庫
        /// </summary>
        /// <param name="signUpParameter">包含密碼、email、暱稱的物件</param>
        /// <returns>被新增的筆數</returns>
        public int SignUp(SignUpParameter signUpParameter)
        {
            NomadicBookContext.UserDatas.Add(new UserData()
            {
                NickName = signUpParameter.NickName,
                Password = Security.Encryption(signUpParameter.Password),
                Email = signUpParameter.Email,
                TotalEvaluation=0,
                TradeNum=0,
                Verification=false
            });
            var result = NomadicBookContext.SaveChanges();
            return result;

            }
            /// <summary>
            /// 取得使用者資料裡存的預設的寄送地址
            /// </summary>
            /// <param name="userId">使用者id</param>
            /// <returns>預設的寄送地址</returns>
            public PresetAddressDto GetPresetAddress(short userId) 
        {
            var presetAddress= NomadicBookContext.UserDatas.Where(user => user.UserId == userId).Select(user=>new PresetAddressDto 
            {
                StoreAddress= ReplaceNull(user.StoreAddress),
                StoreName= ReplaceNull(user.StoreName),
                MailBoxAddress= ReplaceNull(user.MailBoxAddress),
                MailBoxName= ReplaceNull(user.MailBoxName),
                HomeAddress= ReplaceNull(user.HomeAddress),
                FaceTradeCity= ReplaceNull(user.FaceTradeCity),
                FaceTradeArea= ReplaceNull(user.FaceTradeArea),
                FaceTradeRoad= ReplaceNull(user.FaceTradeRoad),
                FaceTradePath= ReplaceNull(user.FaceTradePath),
                FaceTradeDetail= ReplaceNull(user.FaceTradeDetail),
                TrueName= ReplaceNull(user.TrueName),
                CellphoneNumber= ReplaceNull(user.CellphoneNumber)
            }).SingleOrDefault();
            return presetAddress;
        }
        /// <summary>
        /// 用使用者最新的上架資料修改使用者的預設寄送地址
        /// </summary>
        /// <param name="bookData">使用者這次的上架資料</param>
        public void SetPresetAddress(BookParameter bookData)
        {
            var user = NomadicBookContext.UserDatas.SingleOrDefault(user => user.UserId == bookData.UserId);
            if (user == null)
            {
                return;
            }
            if (bookData.StoreAddress != null)
            {
                user.StoreAddress = bookData.StoreAddress;
                user.StoreName = bookData.StoreName;
            }
            if (bookData.MailBoxAddress != null)
            {
                user.MailBoxAddress = bookData.MailBoxAddress;
                user.MailBoxName = bookData.MailBoxName;
            }
            if (bookData.HomeAddress!=null)
            {
                user.HomeAddress = bookData.HomeAddress;
            }
            if (bookData.FaceTradeCity != null)
            {
                user.FaceTradeCity = bookData.FaceTradeCity;
                user.FaceTradeArea = bookData.FaceTradeArea;
                user.FaceTradeRoad = bookData.FaceTradeRoad;
                user.FaceTradePath = bookData.FaceTradePath;
                user.FaceTradeDetail = bookData.FaceTradeDetail;
            }
            if (bookData.TrueName != null)
            {
                user.TrueName = bookData.TrueName;
            }
            if (bookData.CellphoneNumber != null)
            {
                user.CellphoneNumber = bookData.CellphoneNumber;
            }
            NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 給使用者做更改密碼的動作
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <param name="user">使用者要更改的密碼資料</param>
        /// <returns>回傳Status Code</returns>
        public int SetPassword(short userId, PasswordParameter user)
        {
            string password =Security.Encryption(user.OldPassword);
            var userData = NomadicBookContext.UserDatas.SingleOrDefault(user => user.UserId == userId && user.Password == password);
            if (userData != null)
            {
                userData.Password= Security.Encryption(user.NewPassword);
            }
            return NomadicBookContext.SaveChanges();

        }
        /// <summary>
        /// 使用者忘記密碼時，產生一組亂碼作為新密碼存入資料庫並寄給使用者
        /// </summary>
        /// <param name="email">使用者信箱</param>
        /// <returns>資料庫被改動的值</returns>
        public int ForgetPassword( string email)
        {
            var user = NomadicBookContext.UserDatas.Where(user => user.Email == email).SingleOrDefault();
            if (user != null)
            {
                string password = RandomPassword();
                user.Password = Security.Encryption(password);
                Mail.SendMail(email, "忘記密碼", $"新的密碼為{password}，請在登入後一天內重新設定密碼");
            }
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 產生亂碼
        /// </summary>
        /// <returns>亂碼</returns>
        public string RandomPassword()
        {
            Random random = new Random();
            string password =string.Empty;
            password += (char)random.Next(48, 57);//密碼一定要包含數字和英文
            password += (char)random.Next(97, 122);
            for (int i = 0; i < 6; i++)
            {
                int num= random.Next(1, 3);
                switch (num)
                {
                    case 1:
                        password += (char)random.Next(48, 57);
                        break;
                    case 2:
                        password += (char)random.Next(65, 90);
                        break;
                    case 3:
                        password += (char)random.Next(97, 122);
                        break;
                }
            }
            return password;
        }
        public string Authenticate(UserData user)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim( JwtRegisteredClaimNames.Sub,user.NickName));// User.Identity.Name
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // JWT ID
            //建立 JWT TokenHandler 以及用於描述 JWT 的 TokenDescriptor
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(IConst.SecurityKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = Audiences.UpdateAudience(user.NickName),
                Subject = new ClaimsIdentity(claims),//建立使用者的 Claims 聲明，這會是 JWT Payload 的一部分
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor); //產出所需要的 JWT Token 物件
            var tokenString = tokenHandler.WriteToken(token);//產出序列化的 JWT Token 字串
            return tokenString;
        }
        /// <summary>
        /// 驗證是否帳號已用EMAIL開通
        /// </summary>
        /// <param name="userId">使用者ID</param>
        /// <returns>是否開通</returns>
        public bool IsVerify(short userId)
        {
            return NomadicBookContext.UserDatas.SingleOrDefault(user => user.UserId == userId).Verification;
        }
        /// <summary>
        /// 寄送帳號開通信件
        /// </summary>
        /// <param name="email">要寄的EMAIL</param>
        public void SendVerify(string email)
        {
            string aesEmail = Security.Base64Encrypt(email, new UTF8Encoding()); 
            Mail.SendMail(email, "帳號驗證", $@"<a href='http://possible-arbor-315613.appspot.com/#/notifications/{aesEmail}'>請點擊連結驗證帳號</a>");
        }
        /// <summary>
        /// 附在帳號開通信中，讓人點擊開通帳號的方法http://35.236.167.85
        /// </summary>
        /// <param name="aesEmail">使用者EMAIL</param>
        public void Verify(string aesEmail)
        {
            string email = Security.Base64Decrypt(aesEmail, new UTF8Encoding());
            var user= NomadicBookContext.UserDatas.SingleOrDefault(user => user.Email.Equals(email));
            if (user != null)
            {
                user.Verification = true;
                NomadicBookContext.SaveChanges();
            }
        }
    }
}
