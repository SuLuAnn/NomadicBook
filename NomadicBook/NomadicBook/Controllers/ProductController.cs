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
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IProductService ProductService;
        private readonly IPhotoService PhotoService;
        private readonly Photo PhotoUpload; 
        private readonly ISignInService SignInService;
        /// <summary>
        /// 注入產品服務
        /// </summary>
        /// <param name="productService">產品服務</param>
        public ProductController(IProductService productService, Photo photoUpload, ISignInService signInService, IPhotoService photoService) 
        {
            ProductService = productService;
            PhotoUpload = photoUpload;
            SignInService = signInService;
            PhotoService = photoService;
        }
        /// <summary>
        /// 回傳取得的產品列表
        /// </summary>
        /// <param name="mainId">主類別id</param>
        /// <param name="bigCategory">大類名稱</param>
        /// <param name="categoryId">類別id</param>
        /// <returns>回傳Status Code，200是成功取得，裡面包產品清單集合</returns>
        // GET: api/<ProductController>
        [HttpGet("list")]
        public ActionResult Get([FromQuery] byte mainId, string bigCategory,string categoryId,int max,string keyWord)
        {
            var productList = ProductService.GetProductList(mainId, bigCategory,categoryId, max, keyWord);
            if (productList.Count()==0)
            {
                return NotFound();
            }
            return Ok(productList);
        }
        /// <summary>
        /// 回傳產品資訊
        /// </summary>
        /// <param name="bookId">產品id</param>
        /// <returns>回傳Status Code，200是成功取得，404是id不存在</returns>
        // GET api/<ProductController>/5
        [HttpGet("{bookId}")]
        public ActionResult Get(int bookId)
        {
            var product = ProductService.GetProduct(bookId);
            if (product == null) 
            {
                return NotFound("查無資料");
            }
            return Ok(product);
        }
        /// <summary>
        /// 上架書本
        /// </summary>
        /// <param name="bookData">上架資訊</param>
        /// <returns>回傳Status Code，200是成功上架</returns>
        // POST api/<ProductController>
        [Authorize]
        [HttpPost("new")]
        public async Task<ActionResult> Post([FromForm] BookParameter bookData)
        {
            List<string> prompts = new List<string>();
            prompts.Add(Global.IsCorrectLength(bookData.Isbn, StringLimit.ISBN));
            prompts.Add(Global.IsCorrectLength(bookData.BookName, StringLimit.BookName));
            prompts.Add(Global.IsCorrectLength(bookData.Author, StringLimit.Author));
            prompts.Add(Global.IsCorrectLength(bookData.PublishingHouse, StringLimit.PublishingHouse));
            prompts.Add(Global.IsCorrectLength(bookData.Experience, StringLimit.Experience));
            prompts.Add(Global.IsCorrectLength(bookData.Introduction, StringLimit.Introduction));
            prompts.Add(Global.IsCorrectLength(bookData.Condition, StringLimit.Condition));
            prompts.Add(Global.IsCorrectLength(bookData.HomeAddress, StringLimit.Address));
            prompts.Add(Global.IsCorrectLength(bookData.FaceTradePath, StringLimit.Path));
            prompts.Add(Global.IsCorrectLength(bookData.FaceTradeDetail, StringLimit.Detail));
            prompts.Add(Global.IsCorrectLength(bookData.TrueName, StringLimit.TrueName));
            prompts.Add(Global.IsCorrectLength(bookData.CellphoneNumber, StringLimit.CellphoneNumber));
            foreach (var prompt in prompts) 
            {
                if (prompt != null)
                {
                    return NotFound(prompt);
                }
            }
            var photos = bookData.BookPhoto;
            if (photos == null)
            {
                return NotFound("圖片至少需一張");
            }
            int bookId = ProductService.UploadBook(bookData);
            if (bookId == 0)
            {
                return NotFound("上架失敗");
            }
            List<string> photoPaths = new List<string>();
            int i = 0;
            foreach (var photo in photos)
            {
                string fileName = $"{DateTime.UtcNow.AddHours(08).ToString("yyyyMMddHHmmss")}{bookId}{i++}";
                photoPaths.Add(fileName);
                await PhotoUpload.Upload(photo, fileName);
            }
            if (PhotoService.UploadPhoto(bookId, photoPaths) == 0)
            {
                return NotFound("圖片上架失敗");
            }
            SignInService.SetPresetAddress(bookData);
            return Ok(bookId);
        }
        /// <summary>
        /// 使用者用isbn可以從博客來取得這本書的詳細資料
        /// </summary>
        /// <param name="isbn">isbn編號，應該會是10碼或13碼</param>
        /// <returns>含有書本資訊的Status Code</returns>
        [HttpGet]
        public ActionResult GetByIsbn([FromQuery] string isbn)
        {
            string prompt = Global.IsCorrectLength(isbn, StringLimit.ISBN);
            if (prompt != null)
            {
                return NotFound(prompt);
            }
            var product = ProductService.GetByIsbnDatabase(isbn);
            if (product != null)
            {
                return Ok(product);
            }
            product = ProductService.GetByIsbn(isbn);
            if (product != null)
            {
                return Ok(product);
            }
            return NotFound("isbn查無資料");
        }
        /// <summary>
        /// 取得BookCategory的資料
        /// </summary>
        /// <param name="url">博客來分類種覽的URL</param>
        /// <param name="mainId">要取得的大類ID，可見enum MainCategory</param>
        /// <returns>Status Code</returns>
        [HttpPost("category/all")]
        public ActionResult Post(string url, byte mainId)
        {
            ProductService.GetAllCategory(url, mainId);
            return Ok();
        }
        /// <summary>
        /// 取得該主類下的所有大類名稱
        /// </summary>
        /// <param name="mainId">主類id</param>
        /// <returns>Status Code+所有大類名稱</returns>
        [HttpGet("category")]
        public ActionResult GetBigCategory([FromQuery] byte mainId)
        {
            var bigCategory = ProductService.GetBigCategory(mainId);
            if (bigCategory.Count > 0)
            {
                return Ok(bigCategory);
            }
            return NotFound("主類別id錯誤");
        }
        /// <summary>
        /// 取得該主類的該大類名稱下的所有類別id及類別名稱
        /// </summary>
        /// <param name="mainId">主類id</param>
        /// <param name="bigName">大類名稱</param>
        /// <returns>Status Code+所有類別id及類別名稱</returns>
        [HttpGet("category/detail")]
        public ActionResult GetDetailCategory([FromQuery] byte mainId,string bigName)
        {
            var detailCategory = ProductService.GetDetailCategory(mainId,bigName);
            if (detailCategory.Count > 0)
            {
                return Ok(detailCategory);
            }
            return NotFound();
        }
        /// <summary>
        /// 用小分類ID取得大分類和主分類，主要是ISBN自動填入的時候需要知道他的大分類和主分類
        /// </summary>
        /// <param name="categoryId">小分類ID</param>
        /// <returns>Status Code+該小分類的大分類和主分類</returns>
        [HttpGet("category/belong")]
        public ActionResult GetBelongCategory([FromQuery] string categoryId)
        {
            var belongCategory = ProductService.GetBelongCategory(categoryId);
            if (belongCategory!=null)
            {
                return Ok(belongCategory);
            }
            return NotFound();
        }
        /// <summary>
        /// 確認該名使用者有無選過這本書
        /// </summary>
        /// <param name="userId">使用者ID</param>
        /// <param name="bookId">書ID</param>
        /// <returns>Status Code+是否選過</returns>
        [Authorize]
        [HttpGet("book/chosen")]
        public ActionResult IsChosen([FromQuery] short userId, [FromQuery] int bookId)
        {
            return Ok(ProductService.IsChosen(userId, bookId));
        }
    }
}
