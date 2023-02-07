using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Utils
{
    public class Photo
    {
        /// <summary>
        /// 上傳圖片到雲端
        /// </summary>
        /// <param name="photo">要上船的圖檔</param>
        /// <param name="fileName">預計的檔名</param>
        /// <returns></returns>
        public async Task Upload(IFormFile photo,string fileName)
        {
            string path = @$"photo/{fileName}.jpg";
            using (var strearm = new FileStream(path, FileMode.Create))
            {
                await photo.CopyToAsync(strearm);
            }
        }
        /// <summary>
        /// 刪除指定圖檔
        /// </summary>
        /// <param name="fileName">要刪除的檔名</param>
        public void Delete(string fileName)
        {
            string path = @$"photo/{fileName}.jpg";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
