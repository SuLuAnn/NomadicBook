using System;
using System.Collections.Generic;

#nullable disable

namespace NomadicBook.Models.db
{
    public partial class ConvenienceStore
    {
        public int ShopId { get; set; }
        public string ShopName { get; set; }
        public string ShopCity { get; set; }
        public string ShopArea { get; set; }
        public string ShopAddress { get; set; }
    }
}
