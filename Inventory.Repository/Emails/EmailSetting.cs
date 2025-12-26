using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Repository.Emails
{
    public static class EmailSetting
    {
        public static void SendEmail(Email email)
        {
            var Client = new SmtpClient("smtp.gmail.com", 587);
            Client.EnableSsl = true;
            Client.Credentials = new NetworkCredential("dhammad339@gmail.com", "qetcoulrrdlfyzvn");
            Client.Send("dhammad339@gmail.com", email.To, email.Subject, email.Body);
        }
    }
}
