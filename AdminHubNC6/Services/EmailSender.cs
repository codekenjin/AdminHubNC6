using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace InfoHub.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var message = new MailMessage())
            {
                message.To.Add(new MailAddress(email, email));
                message.From = new MailAddress("Sender Email <sender@sender.com>", "Sender Email Here");
                message.Subject = subject;
                message.Body = htmlMessage;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    //var credential = new NetworkCredential
                    //{
                    //    UserName = "user@outlook.com",  // replace with valid value
                    //    Password = "password"  // replace with valid value
                    //};
                    //smtp.Credentials = credential;
                    //smtp.Port = 25;
                    //smtp.EnableSsl = true;
                    smtp.Host = "mail2.edb.gov.hk"; // unquote for production
                    smtp.Send(message);
                }
            }

            return Task.CompletedTask;
        }
    }
}