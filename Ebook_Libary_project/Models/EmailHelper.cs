using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Net.Configuration;
using System.Threading.Tasks;



namespace Ebook_Libary_project.Models
{
    public class EmailHelper
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPass;
        private readonly bool _enableSsl;

        public EmailHelper(string smtpHost, int smtpPort, string smtpUser, string smtpPass, bool enableSsl)
        {
            _smtpHost = smtpHost;
            _smtpPort = smtpPort;
            _smtpUser = smtpUser;
            _smtpPass = smtpPass;
            _enableSsl = enableSsl;
        }
        //Sending an email opertations.
        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                //Getting Email info
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                mailMessage.To.Add(toEmail);
                //sends the email.
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(_smtpUser, _smtpPass);
                    smtpClient.EnableSsl = _enableSsl;

                    await smtpClient.SendMailAsync(mailMessage);
                }

                return true; // Email sent successfully,Yay!
            }
            catch
            {
                // Log or handle exceptions as needed
                return false; // Email sending failed
            }
        }
    }
}
    
