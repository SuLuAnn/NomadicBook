using NomadicBook.Dto;
using NomadicBook.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Service
{
    public interface IStallService
    {
        public List<StallBookDto> GetStallBooks(short userId);
        public int PutOnBook(int bookId);
        public int PutOffBook(int bookId);
        public int UpdateBook(int bookId, BookUpdateParameter book);
    }
}
