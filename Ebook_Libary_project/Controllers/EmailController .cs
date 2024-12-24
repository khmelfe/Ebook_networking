using System.Web.Helpers;
using System.Web.Mvc;
using System;

namespace Ebook_Libary_project.Controllers
{
    public class EmailController : Controller
    {
        public static void SendEmail()
        {
            string emailadd = "filex002@gmail.com";
            string subject = "Hello sire";
            string body = "IT'S time";

            try
            {
                // Send the email
                WebMail.Send(emailadd, subject, body);

                // Optionally, send a success message
               // return Content("Email sent successfully.");
            }
            catch (Exception ex)
            {
                // Log and return the error message
                //return Content("Error sending email: " + ex.Message);
            }
        }
    }
}
