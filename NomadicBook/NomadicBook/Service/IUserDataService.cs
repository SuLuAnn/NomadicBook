using NomadicBook.Dto;
using NomadicBook.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Service
{
    public interface IUserDataService
    {
        public BasicUserDto GetBasicData(short userId);
        public DetailUserDto GetDetailData(short userId);
        public (int,string, string) Update(short userId, UserParameter user);
    }
}
