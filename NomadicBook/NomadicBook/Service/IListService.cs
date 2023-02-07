using NomadicBook.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Service
{
    public interface IListService
    {
        public List<ExperienceListDto> GetExperienceList(int max);
        public List<ListProductDto> GetPublishDayList(int max);

        public List<ListProductDto> GetNewBook(int max);
    }
}
