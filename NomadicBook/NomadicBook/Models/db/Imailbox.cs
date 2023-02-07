using System;
using System.Collections.Generic;

#nullable disable

namespace NomadicBook.Models.db
{
    public partial class Imailbox
    {
        public short MailboxId { get; set; }
        public string MailboxName { get; set; }
        public string MailboxCity { get; set; }
        public string MailboxArea { get; set; }
        public string MailboxAddress { get; set; }
    }
}
