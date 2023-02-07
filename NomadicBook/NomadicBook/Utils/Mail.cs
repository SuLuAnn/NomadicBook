using System.Net.Mail;

namespace NomadicBook.Utils
{
    public class Mail
    {
        public static void SendMail(string email,string header,string message) 
        {
            message = @$"<h3>{message}</h3><br><br><a href='http://possible-arbor-315613.appspot.com'><img src = 'http://35.236.167.85/photo/NomadicBook.png' alt = 'NomadicBook' width = '400px' height = '222px' border='0'></a>";
                         MailMessage msg = new MailMessage();
            msg.To.Add(email); 
            msg.From = new MailAddress("{這邊要填email}", "遊牧書籍客服", System.Text.Encoding.UTF8);
        /* 上面3個引數分別是發件人地址，發件人姓名，編碼*/
            msg.Subject = header;//郵件標題 
            msg.SubjectEncoding = System.Text.Encoding.UTF8;//郵件標題編碼 
            msg.Body = message;//郵件內容 
            msg.BodyEncoding = System.Text.Encoding.UTF8;//郵件內容編碼 
            msg.IsBodyHtml =true;//是否是HTML郵件
            msg.Priority = MailPriority.High;//郵件優先順序 
            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("{這邊要填email}", "{emai密碼}");
            //上述寫你的GMail郵箱和密碼 
            client.Port = 587;//Gmail使用的埠 
            client.Host = "smtp.gmail.com"; 
            client.EnableSsl =true;//經過ssl加密
            client.Send(msg);
        }
    }
}
