using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NomadicBook.Parameters;
using NomadicBook.Service;
using NomadicBook.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using static NomadicBook.Utils.Global;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NomadicBook.Controllers
{
    /// <summary>
    /// 注入註冊服務
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ISignInService SignInService;
        private readonly IUserDataService UserDataService;
        private readonly Photo Photo;
        public UserController(ISignInService signInService, IUserDataService userDataService, Photo photo)
        {
            SignInService = signInService;
            UserDataService = userDataService;
            Photo = photo;
        }
        /// <summary>
        /// 確認暱稱是否重複
        /// </summary>
        /// <param name="nickname">暱稱</param>
        /// <returns>回傳Status Code，200是沒被使用，404是已被使用</returns>
        // GET: api/<UserController>
        [HttpGet("name")]
        public ActionResult GetCheckNickName([FromQuery] string nickname)
        {
            var canUse = SignInService.CanUseNickName(nickname);
            if (canUse)
            {
                return Ok(canUse);
            }
            return NotFound("此暱稱已被使用");
        }
        /// <summary>
        /// 確認email是否重複
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>回傳Status Code，200是沒被使用，404是已被使用</returns>
        // GET api/<UserController>
        [HttpGet("mail")]
        public ActionResult GetCheckMail([FromQuery] string email)
        {
            var canUse = SignInService.CanUseEmail(email);
            if (canUse)
            {
                return Ok(canUse);
            }
            return NotFound("此email已被註冊過");
        }
        /// <summary>
        /// 使用者登入確認密碼、email正確
        /// </summary>
        /// <param name="signIn">存放密碼、email的物件</param>
        /// <returns>回傳Status Code，200是登入成功，404是登入失敗，403是email未驗證</returns>
        //POST api/<UserController>
        [HttpPost("signin")]
        public ActionResult Post([FromBody] SignInParameter signIn)
        {
            var signInData = SignInService.GetSignInData(signIn);
            if (signInData == null)
            {
                return NotFound("密碼或email有誤");
            }
            bool IsVerify = SignInService.IsVerify(signInData.UserId);
            if (IsVerify)
            {
                return Ok(signInData);
            }
            return Forbid();

        }
        /// <summary>
        /// 使用者註冊
        /// </summary>
        /// <param name="signUpParameter">包含暱稱、密碼、email</param>
        /// <returns>回傳Status Code，200是註冊成功，404是註冊失敗</returns>
        // POST api/<UserController>
        [HttpPost("signup")]
        public ActionResult Post([FromBody] SignUpParameter signUpParameter)
        {
            List<string> prompts = new List<string>();
            prompts.Add(Global.IsCorrectLength(signUpParameter.NickName, StringLimit.NickName));
            prompts.Add(Global.IsCorrectLength(signUpParameter.Password, StringLimit.Password));
            prompts.Add(Global.IsCorrectLength(signUpParameter.Email, StringLimit.Email));
            foreach (var prompt in prompts)
            {
                if (prompt != null)
                {
                    return NotFound(prompt);
                }
            }
            int addResult = SignInService.SignUp(signUpParameter);
            if (addResult > 0)
            {
                SignInService.SendVerify(signUpParameter.Email);
                return Ok("新增成功");
            }
            return NotFound("新增失敗");

        }
        /// <summary>
        /// 取得使用者資料裡存的預設的寄送地址
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <returns>回傳Status Code+預設的寄送地址</returns>
        // GET api/<UserController>
        [Authorize]
        [HttpGet("presetaddress/{userId}")]
        public ActionResult GetPresetAddress(short userId)
        {
            var presetAddress = SignInService.GetPresetAddress(userId);
            return Ok(presetAddress);
        }
        /// <summary>
        /// 回傳使用者基本資料
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <returns>回傳Status Code+基本資料</returns>
        // GET api/<UserController>
        [HttpGet("basicdata/{userId}")]
        public ActionResult GetBasicData(short userId)
        {
            var user = UserDataService.GetBasicData(userId);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound("使用者id不存在");
        }
        /// <summary>
        /// 取得使用者詳細資料
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <returns>回傳Status Code+使用者詳細資料</returns>
        // GET api/<UserController>
        [Authorize]
        [HttpGet("detaildata/{userId}")]
        public ActionResult GetDetailData(short userId)
        {
            var user = UserDataService.GetDetailData(userId);
            if (user != null)
            {
                return Ok(user);
            }
            return NotFound("使用者id不存在");
        }
        /// <summary>
        /// 修改使用者詳細資料
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <param name="user">要更新的使用者資料</param>
        /// <returns>回傳Status Code</returns>
        // PUT api/<ValuesController>/5
        [Authorize]
        [HttpPut("{userId}")]
        public async Task<ActionResult> Put(short userId, [FromForm] UserParameter user)
        {
            List<string> prompts = new List<string>();
            prompts.Add(Global.IsCorrectLength(user.NickName, StringLimit.NickName));
            prompts.Add(Global.IsCorrectLength(user.SelfIntroduction, StringLimit.SelfIntroduction));
            prompts.Add(Global.IsCorrectLength(user.HomeAddress, StringLimit.Address));
            prompts.Add(Global.IsCorrectLength(user.FaceTradePath, StringLimit.Path));
            prompts.Add(Global.IsCorrectLength(user.FaceTradeDetail, StringLimit.Detail));
            prompts.Add(Global.IsCorrectLength(user.TrueName, StringLimit.TrueName));
            prompts.Add(Global.IsCorrectLength(user.CellphoneNumber, StringLimit.CellphoneNumber));
            foreach (var prompt in prompts)
            {
                if (prompt != null)
                {
                    return NotFound(prompt);
                }
            }
            (int num, string photoName, string oldName) result = UserDataService.Update(userId, user);
            if (result.num > 0)
            {
                if (result.photoName != null)
                {
                    Photo.Delete(result.oldName);
                    await Photo.Upload(user.UserPhoto, result.photoName);
                }
                return Ok("會員資料已更新");
            }
            return NotFound("會員資料更新失敗");
        }
        /// <summary>
        /// 給使用者做更改密碼的動作
        /// </summary>
        /// <param name="userId">使用者id</param>
        /// <param name="user">使用者要更改的密碼資料</param>
        /// <returns>回傳Status Code</returns>
        // PUT api/<ValuesController>/5
        [Authorize]
        [HttpPut("password/{userId}")]
        public ActionResult PutPassword(short userId, [FromBody] PasswordParameter user)
        {
            string prompt = Global.IsCorrectLength(user.NewPassword, StringLimit.Password);
            if (prompt != null)
            {
                return NotFound(prompt);
            }
            if (SignInService.SetPassword(userId, user) > 0)
            {
                return Ok("密碼已修改");
            }
            return NotFound("密碼修改失敗");
        }
        /// <summary>
        /// 使用者忘記密碼是random一組新密碼並發email給使用者
        /// </summary>
        /// <param name="email">email</param>
        /// <returns>回傳Status Code</returns>
        // PUT api/<ValuesController>/5
        [HttpPut("forgetpassword")]
        public ActionResult PutPassword([FromBody] string email)
        {
            int result = SignInService.ForgetPassword(email);
            if (result > 0)
            {
                return Ok("密碼已寄至email");
            }
            return NotFound("email錯誤");
        }
        /// <summary>
        /// 寄送帳號開通信件
        /// </summary>
        /// <param name="email">要寄的EMAIL</param>
        /// <returns>回傳Status Code</returns>
        [HttpGet("verifymail/{email}")]
        public ActionResult Get(string email)
        {
            SignInService.SendVerify(email);
            return Ok("驗證信已寄出");
        }

        /// <summary>
        /// 附在帳號開通信中，讓人點擊開通帳號的方法
        /// </summary>
        /// <param name="email">使用者EMAIL</param>
        [HttpGet("verify/{email}")]
        public void GetVerify(string email)
        {
            SignInService.Verify(email);
        }
    }
}
