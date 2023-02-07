using NomadicBook.Dto;
using NomadicBook.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static NomadicBook.Utils.Global;

namespace NomadicBook.Service
{
    public interface ISeekService
    {
        public int AddNewSeek(NewSeekParameter newSeek);
        public List<MySeekDto> GetMySeek(short userId);
        public List<OtherSeekDto> GetOtherSeek(short userId);
        public List<ListProductDto> GetOtherBook(short userId,byte tradeMode);
        public int RespondSeek(int seekId, RespondParameter respondSeek);
        public int RefusalSeek(int seekId, short userId);
        public List<MatchDto> GetMatches(short userId, SeekStatus status);
        public int BookSend(int seekId, short userId);
        public int BookReceive(int seekId, short userId);
        public SeekExistDto SeekExist(short userId, short stallUserId);
        public int SetEvaluation(int seekId, EvaluationParameter evaluation);
        public void SetUserEvaluation(short userId, double evaluation);
    }
}
