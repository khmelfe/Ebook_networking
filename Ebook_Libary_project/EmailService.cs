using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;
using System.Net.Configuration;

public class EmailService
{
    public void SendEmail(string toEmail, string subject, string body)
    {
        try
        {
            // Retrieve SMTP settings from web.config
            var smtpSection = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;

            if (smtpSection == null)
                throw new Exception("SMTP settings are missing in the web.config file.");

            using (var client = new SmtpClient(smtpSection.Network.Host, smtpSection.Network.Port))
            {
                client.Credentials = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);
                client.EnableSsl = smtpSection.Network.EnableSsl;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSection.From),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Set to true if sending HTML content
                };

                mailMessage.To.Add(toEmail);

                client.Send(mailMessage);
            }
        }
        catch (Exception ex)
        {
            // Log exception or handle as needed
            throw new Exception("An error occurred while sending the email: " + ex.Message);
        }
    }
}
