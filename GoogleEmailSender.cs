using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace WahsKeyClubSite
{
    public class GoogleEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var mail = new MailMessage("wahskeyclubnoreply@gmail.com", email);
            var client = new SmtpClient
            {
                Port = 587,
                Host = "smtp.gmail.com",
                Credentials = new NetworkCredential("wahskeyclubnoreply@gmail.com", "mrgjzgllcssiboar"),
                EnableSsl = true
            };
            mail.Subject = subject;
            mail.Body = htmlMessage + "\n" + "\n" + "\n" + "Please do not reply to these emails; they are automated and we cannot see your replies. If you want to contact us, do so at wahskeyclub13@gmail.com.";
            
            return client.SendMailAsync(mail);
        }
    }
}