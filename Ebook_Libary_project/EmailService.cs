using System;
using System.Net;
using System.Net.Mail;

public class EmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587;
    private readonly string _senderEmail = "mailprojects827@gmail.com"; // Replace with your Gmail address
    private readonly string _senderPassword = "Fnyeax14825"; // Replace with your Gmail password or App password

    public void SendEmail(string toEmail, string subject, string body)
    {
        try
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_senderEmail, _senderPassword);

                var mailMessage = new MailMessage(_senderEmail, toEmail, subject, body);
                client.Send(mailMessage);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to send email: " + ex.Message);
        }
    }
}
