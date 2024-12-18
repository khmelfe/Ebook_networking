using System;
using System.Net;
using System.Net.Mail;
using System.Configuration;

public class EmailService
{
    public void SendEmail(string toEmail, string subject, string body)
    {
        // Get SMTP settings from web.config
        string smtpHost = ConfigurationManager.AppSettings["SmtpHost"];
        int smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
        string smtpUser = ConfigurationManager.AppSettings["SmtpUser"];
        string smtpPass = ConfigurationManager.AppSettings["SmtpPass"];
        bool enableSsl = bool.Parse(ConfigurationManager.AppSettings["EnableSsl"]);

        try
        {
            // Create a new MailMessage object
            using (SmtpClient smtp = new SmtpClient(smtpHost, smtpPort))
            {
                smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtp.EnableSsl = enableSsl;

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(smtpUser),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Allows HTML content in email body
                };
                mail.To.Add(toEmail);

                // Send the email
                smtp.Send(mail);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to send email", ex);
        }
    }
}
