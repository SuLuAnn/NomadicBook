using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Service
{
    public interface IPhotoService
    {
        public int UploadPhoto(int bookId, List<string> photoPaths);
        public string DeletePhoto(int photoId);
    }
}
