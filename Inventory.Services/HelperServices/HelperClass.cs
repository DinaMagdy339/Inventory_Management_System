using Microsoft.AspNetCore.Http;
using System.Net.Mail;

namespace Inventory.Services.HelperServices
{
    public class HelperClass
    {
        public async Task SendEmail(string FromEmail, string Subject, string FromName, string Message, string ToEmail, string ToName,
        string smtpMessage, string smtpPassword, string smtpHost, int smtpPort, bool smtpSSl)
        {
            var body = Message;
            var messageRequest = new MailMessage();
            messageRequest.To.Add(new MailAddress(ToEmail, ToName));
            messageRequest.From = new MailAddress(FromEmail, FromName);
            messageRequest.Subject = Subject;
            messageRequest.Body = body;
            messageRequest.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                var credential = new System.Net.NetworkCredential
                {
                    UserName = smtpMessage,
                    Password = smtpPassword
                };
                smtp.Credentials = credential;
                smtp.Host = smtpHost;
                smtp.Port = smtpPort;
                smtp.EnableSsl = smtpSSl;
                await smtp.SendMailAsync(messageRequest);
            }

        }

        public async Task<string> UploadFile(List<IFormFile> Files, IWebHostEnvironment env, string uploadFolder)
        {
            var responce = string.Empty;
            var uploadPath = Path.Combine(env.WebRootPath, uploadFolder);
            var FileExension = string.Empty;
            var FilePath = string.Empty;
            var FileName = string.Empty;
            foreach (var file in Files)
            {
                if (file.Length > 0)
                {
                    FileExension = Path.GetExtension(file.FileName);
                    FileName = Guid.NewGuid().ToString() + FileExension;
                    FilePath = Path.Combine(uploadPath, FileName);
                    using (var stream = new FileStream(FilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    responce = FileName;

                }
            }
            return responce;
        }


    }
}
