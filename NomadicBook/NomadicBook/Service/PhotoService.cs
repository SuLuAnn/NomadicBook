using NomadicBook.Models.db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Service
{
    public class PhotoService: IPhotoService
    {
        private readonly NomadicBookContext NomadicBookContext;
        public PhotoService(NomadicBookContext nomadicBookContext)
        {
            NomadicBookContext = nomadicBookContext;
        }
        /// <summary>
        /// 將使用者傳入的圖片路徑上傳資料庫
        /// </summary>
        /// <param name="bookId">圖片所屬書本id</param>
        /// <param name="photoPaths">圖片路徑集合</param>
        public int UploadPhoto(int bookId, List<string> photoPaths)
        {
            foreach (string path in photoPaths)
            {
                NomadicBookContext.BookPhotoes.Add(new BookPhoto
                {
                    BookId = bookId,
                    BookPhoto1 = path
                });
            }
            return NomadicBookContext.SaveChanges();
        }
        /// <summary>
        /// 從資料庫刪除指定圖檔
        /// </summary>
        /// <param name="photoId"></param>
        /// <returns>圖檔檔名</returns>
        public string DeletePhoto(int photoId)
        {
            var photo= NomadicBookContext.BookPhotoes.SingleOrDefault(photo => photo.BookPhotoId == photoId);
            if (photo != null)
            {
                NomadicBookContext.BookPhotoes.Remove(photo);
            }
            NomadicBookContext.SaveChanges();
            return photo.BookPhoto1;
        }
    }
}
