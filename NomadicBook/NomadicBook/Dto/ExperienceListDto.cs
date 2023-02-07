using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NomadicBook.Dto
{
    public class ExperienceListDto
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public short UserId { get; set; }
        public string UserName { get; set; }
        public string Author { get; set; }
        public string BookPhoto { get; set; }
        public string Condition { get; set; }
        public byte ConditionNum { get; set; }
        public string Experience { get; set; }
        public string ExperienceDay { get; set; }
    }
}
