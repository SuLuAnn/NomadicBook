using NomadicBook.Dto;
using NomadicBook.Models.db;
using NomadicBook.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Service
{
    public interface IProductService
    {
        public List<ListProductDto> GetProductList(byte mainId, string bigCategory, string categoryId, int max, string keyWord);
        public ProductDto GetProduct(int bookId);
        public  int UploadBook(BookParameter bookData);
        public ISBNDto GetByIsbn(string isbn);
        public void GetAllCategory(string path, byte mainId);
        public List<string> GetBigCategory(byte mainId);
        public List<CategoryDto> GetDetailCategory(byte mainId, string bigName);
        public BelongCategoryDto GetBelongCategory(string detailCategory);
        public bool IsChosen(short userId,int bookId);
        public ISBNDto GetByIsbnDatabase(string isbn);
    }
}
