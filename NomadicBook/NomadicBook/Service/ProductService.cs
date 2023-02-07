using NomadicBook.Dto;
using NomadicBook.Models.db;
using NomadicBook.Parameters;
using NomadicBook.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using static NomadicBook.Utils.Global;

namespace NomadicBook.Service
{
    public class ProductService : IProductService
    {
        private readonly NomadicBookContext NomadicBookContext;

        private readonly IReptile Reptile;
        /// <summary>
        /// 注入資料庫
        /// </summary>
        /// <param name="nomadicBookContext">資料庫</param>
        public ProductService(NomadicBookContext nomadicBookContext, IReptile reptile)
        {
            NomadicBookContext = nomadicBookContext;
            Reptile = reptile;
        }
        /// <summary>
        /// 依據類別ID取得產品列表
        /// </summary>
        /// <param name="mainId">主類別id</param>
        /// <param name="bigCategory">大類名稱</param>
        /// <param name="categoryId">類別id</param>
        /// <returns>產品列表集合</returns>
        public List<ListProductDto> GetProductList(byte mainId, string bigCategory, string categoryId, int max, string keyWord)
        {
            var category = NomadicBookContext.BookCategories.Select(category => category);
            if (mainId != 0)
            {
                category = category.Where(category => mainId == category.MainId);
            }
            if (bigCategory != null)
            {
                category = category.Where(category => bigCategory.Equals(category.BigCategory));
            }
            if (categoryId != null)
            {
                category = category.Where(category => categoryId.Equals(category.CategoryId));
            }
            var books = NomadicBookContext.Books.Where(book => book.BookStatus && book.BookExist);
            if (!String.IsNullOrEmpty(keyWord))
            {
                books=books.Where(book => book.BookName.Contains(keyWord) || book.Author.Contains(keyWord) || book.PublishingHouse.Contains(keyWord));
            }
            var resultList = category.Join(books, category => category.CategoryId, book => book.CategoryId, (category, book) =>
                        new ListProductDto
                        {
                            BookId = book.BookId,
                            BookName = book.BookName,
                            Author = book.Author,
                            BookPhoto = NomadicBookContext.BookPhotoes.Where(photo => book.BookId == photo.BookId).Select(photo => photo.BookPhoto1).FirstOrDefault(),
                            Condition =  ReplaceNull(book.Condition),
                            ConditionNum=book.ConditionNum
                        }
            ).OrderByDescending(book=>book.BookId).AsEnumerable();
            if (max != 0)
            {
                resultList = resultList.Take(max);
            }
            return resultList.ToList();
        }
        /// <summary>
        /// 取得產品頁資料
        /// </summary>
        /// <param name="bookId">產品id</param>
        /// <returns>產品資訊，沒找到回傳null</returns>
        public ProductDto GetProduct(int bookId)
        {
            var book = NomadicBookContext.Books.SingleOrDefault(book => book.BookId == bookId);
            var user = NomadicBookContext.UserDatas.SingleOrDefault(user => user.UserId == book.UserId);
            if (book == null || user==null)
            {
                return null;
            }
            var product = new ProductDto
            {
                BookId = book.BookId,
                UserId = book.UserId,
                UserName = user.NickName,
                Evaluation= ReplaceUndefined(user.TotalEvaluation, user.TradeNum),
                TradeNum=user.TradeNum,
                TrueName= book.TrueName,
                CellphoneNumber= book.CellphoneNumber,
                Isbn =ReplaceNull(book.Isbn),
                BookName = book.BookName,
                Author = book.Author,
                PublishingHouse = book.PublishingHouse,
                CategoryId=book.CategoryId,
                CategoryDetail = NomadicBookContext.BookCategories.Where(category => category.CategoryId == book.CategoryId).
                Select(category => category.DetailCategory).FirstOrDefault(),
                PublishDate = ChangeTime(book.PublishDate),
                BookLong = book.BookLong,
                BookWidth = book.BookWidth,
                BookHigh = book.BookHigh,
                Experience = ReplaceNull(book.Experience),
                Introduction = ReplaceNull(book.Introduction),
                Condition = ReplaceNull(book.Condition),
                ConditionNum = book.ConditionNum,
                StoreAddress = ReplaceNull(book.StoreAddress),
                StoreName = ReplaceNull(book.StoreName),
                MailBoxAddress = ReplaceNull(book.MailBoxAddress),
                MailBoxName = ReplaceNull(book.MailBoxName),
                HomeAddress = ReplaceNull(book.HomeAddress),
                FaceTradeCity = ReplaceNull(book.FaceTradeCity),
                FaceTradeArea = ReplaceNull(book.FaceTradeArea),
                FaceTradeRoad = ReplaceNull(book.FaceTradeRoad),
                FaceTradePath = ReplaceNull(book.FaceTradePath),
                FaceTradeDetail = ReplaceNull(book.FaceTradeDetail),
                BookPhotos = NomadicBookContext.BookPhotoes.Where(photo => book.BookId == photo.BookId).Select(photo => new PhotoDto {
                    BookPhotoId = photo.BookPhotoId,
                    BookPhoto = photo.BookPhoto1
                }).ToList()
            };
            return product;
        }
        /// <summary>
        /// 將使用者傳入的上架資訊傳入資料庫
        /// </summary>
        /// <param name="bookData">上架資訊</param>
        /// <returns>上架那本書自動生成的書本id</returns>
        public int UploadBook(BookParameter bookData)
        {
            var book = new Book
            {
                UserId = bookData.UserId,
                Isbn = bookData.Isbn,
                BookName = bookData.BookName,
                Author = bookData.Author,
                PublishDate = ChangeTime(bookData.PublishDate),
                PublishingHouse = bookData.PublishingHouse,
                CategoryId = bookData.CategoryId,
                BookLong = bookData.BookLong,
                BookWidth = bookData.BookWidth,
                BookHigh = bookData.BookHigh,
                Experience = bookData.Experience,
                Introduction = bookData.Introduction,
                Condition = bookData.Condition,
                ConditionNum = bookData.ConditionNum,
                StoreAddress = bookData.StoreAddress,
                StoreName = bookData.StoreName,
                MailBoxAddress = bookData.MailBoxAddress,
                MailBoxName = bookData.MailBoxName,
                HomeAddress = bookData.HomeAddress,
                FaceTradeCity = bookData.FaceTradeCity,
                FaceTradeArea = bookData.FaceTradeArea,
                FaceTradeRoad = bookData.FaceTradeRoad,
                FaceTradePath = bookData.FaceTradePath,
                FaceTradeDetail = bookData.FaceTradeDetail,
                BookStatus = true,
                BookExist=true,
                ReleaseDate = DateTime.UtcNow.AddHours(08),
                CellphoneNumber = bookData.CellphoneNumber,
                TrueName = bookData.TrueName
            };
            if (book.Experience != null)
            {
                book.ExperienceDay = DateTime.UtcNow.AddHours(08);
            }
            NomadicBookContext.Books.Add(book);
            NomadicBookContext.SaveChanges();
            return book.BookId;
        }
        
        /// <summary>
        /// 使用者用isbn可以從博客來取得這本書的詳細資料
        /// </summary>
        /// <param name="isbn">isbn編號，應該會是10碼或13碼</param>
        /// <returns>書本資訊</returns>
        public ISBNDto GetByIsbn(string isbn)
        {
            string homePage = "https://search.books.com.tw";
            string url = $"{homePage}/search/query/key/{isbn}/cat/BKA";
            var searchPage = Reptile.GetWebPage(url);
            if (searchPage == null)
            {
                return null;
            }
            string bookurl = Reptile.GetHref(searchPage);
            var bookPage = Reptile.GetWebPage(bookurl);
            string dataPath = "body > div.container_24.main_wrap.clearfix > div > div.mod.type02_p01_wrap.clearfix > div.grid_10 > div.type02_p003.clearfix > ul>li";
            var bookData = Reptile.GetBookData(dataPath, bookPage);
            var bookDetail = Reptile.GetBookDetail(bookPage);
            (string company, string date) publishData = Reptile.GetPublishData(bookData);
            (double bookLong, double bookWidth, double bookHigh) size = Reptile.GetSize(bookDetail);
            (string id, string name) category = Reptile.GetCategory(bookDetail);
            string introPath = "body > div.container_24.main_wrap.clearfix > div > div.mod.clearfix > div.grid_19.alpha>div";
            ISBNDto book = new ISBNDto
            {
                BookName = ReplaceNull(Reptile.GetTitle(bookPage)),
                Isbn = isbn,
                Author = ReplaceNull(Reptile.GetAuthor(bookData)),
                PublishingHouse = ReplaceNull(publishData.company),
                PublishDate = ReplaceNull(publishData.date),
                BookLong = size.bookLong,
                BookWidth = size.bookWidth,
                BookHigh = size.bookHigh,
                CategoryId = ReplaceNull(category.id),
                CategoryName = ReplaceNull(category.name),
                Introduction = ReplaceNull(Reptile.GetIntroduction(Reptile.GetBookData(introPath, bookPage)))
            };
            SetIsbnDatabase(book);
            return book;
        }
        public void SetIsbnDatabase(ISBNDto book)
        {
            NomadicBookContext.Isbns.Add(new Isbn
            {
                Isbn1=book.Isbn,
                BookName=book.BookName,
                Author=book.Author,
                PublishingHouse=book.PublishingHouse,
                PublishDate= book.PublishDate,
                BookLong=book.BookLong,
                BookHigh=book.BookHigh,
                BookWidth=book.BookWidth,
                CategoryId=book.CategoryId,
                Introduction=book.Introduction
            });
            NomadicBookContext.SaveChanges();
        }
        public ISBNDto GetByIsbnDatabase(string isbn)
        {
            var book= NomadicBookContext.Isbns.SingleOrDefault(book => book.Isbn1.Equals(isbn));
            if (book == null)
            {
                return null;
            }
            return new ISBNDto
            {
                BookName = book.BookName,
                Isbn = isbn,
                Author = book.Author,
                PublishingHouse = book.PublishingHouse,
                PublishDate = book.PublishDate,
                BookLong = book.BookLong,
                BookWidth = book.BookWidth,
                BookHigh = book.BookHigh,
                CategoryId = book.CategoryId,
                CategoryName = NomadicBookContext.BookCategories.Where(category=>category.CategoryId.Equals(book.CategoryId)).Select(category => category.DetailCategory).SingleOrDefault(),
                Introduction = book.Introduction
            };
        }
        /// <summary>
        /// 將Reptile.GetAllCategory爬回來的中類名稱，小類名稱，及小類id放入資料庫
        /// </summary>
        /// <param name="path">博客來分類種覽的URL</param>
        /// <param name="mainId">要取得的大類ID，可見enum MainCategory</param>
        public void GetAllCategory(string path, byte mainId)
        {
            List<(string id, string bigName, string detailName)> list = Reptile.GetAllCategory(path, mainId);
            foreach (var category in list)
            {
                NomadicBookContext.BookCategories.Add(new BookCategory
                {
                    CategoryId = category.id,
                    MainId = mainId,
                    BigCategory = category.bigName,
                    DetailCategory = category.detailName
                });
                NomadicBookContext.SaveChanges();
            }

        }
        /// <summary>
        /// 取得該主類下的所有大類名稱
        /// </summary>
        /// <param name="mainId">主類id</param>
        /// <returns>所有大類名稱</returns>
        public List<string> GetBigCategory(byte mainId)
        {
            List<string> bigCategory = NomadicBookContext.BookCategories.Where(category => category.MainId == mainId)
                .Select(category => category.BigCategory).Distinct().ToList();
            return bigCategory;
        }
        /// <summary>
        /// 取得該主類的該大類名稱下的所有類別id及類別名稱
        /// </summary>
        /// <param name="mainId">主類id</param>
        /// <param name="bigName">大類名稱</param>
        /// <returns>所有類別id及類別名稱</returns>
        public List<CategoryDto> GetDetailCategory(byte mainId, string bigName)
        {
            List<CategoryDto> detailCategory = NomadicBookContext.BookCategories.Where(category => category.MainId == mainId && category.BigCategory.Equals(bigName))
                .Select(category => new CategoryDto
                {
                    CategoryId = category.CategoryId,
                    DetailCategory = category.DetailCategory
                }).ToList();
            return detailCategory;

        }
        /// <summary>
        /// 用小分類ID取得大分類和主分類，主要是ISBN自動填入的時候需要知道他的大分類和主分類
        /// </summary>
        /// <param name="categoryId">小分類ID</param>
        /// <returns>該小分類的大分類和主分類</returns>
        public BelongCategoryDto GetBelongCategory(string categoryId)
        {
            return NomadicBookContext.BookCategories.Where(category => category.CategoryId == categoryId)
                .Select(category => new BelongCategoryDto
                {
                    MainId = category.MainId,
                    BigCategory = category.BigCategory,
                    MainName= GetDescription((MainCategory)category.MainId)
                }).SingleOrDefault();
        }
        /// <summary>
        /// 確認該名使用者有無選過這本書
        /// </summary>
        /// <param name="userId">使用者ID</param>
        /// <param name="bookId">書ID</param>
        /// <returns>是否選過</returns>
        public bool IsChosen(short userId, int bookId) 
        {
            return NomadicBookContext.SeekRecords
                .Any(seek =>seek.SeekStatus!=(byte)SeekStatus.Cancile&&((seek.SeekBookId == bookId && seek.SeekUserId == userId) || (seek.SeekedBookId == bookId && seek.SeekedUserId == userId)));
        }
    }
}
