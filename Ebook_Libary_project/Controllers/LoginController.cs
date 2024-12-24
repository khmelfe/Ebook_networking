using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Ebook_Libary_project.Models;

namespace Ebook_Libary_project.Controllers
{
    public class LoginController : Controller
    {
        EmailController emailController = new EmailController();

        public ActionResult Login()
        {
            return View(); // Returns the login view to show the form
        }

        [HttpPost]
        public JsonResult Submit(string username, string password)
        {
            // Simple validation for username and password (you can extend it)
            if (username == "fel" && password == "123")
            {
                SendTestEmail();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
        private void SendTestEmail()
        {
            string toEmail = "mailprojects827@gmail.com";
            string subject = "Test Email";
            string body = "This is a test email sent from my MVC application.";

            try
            {
                using (SmtpClient client = new SmtpClient())
                {
                    client.Send("mailprojects827@gmail.com", toEmail, subject, body);
                }
            }
            catch (Exception ex)
            {
                // Handle any errors
                Console.WriteLine("Error sending email: " + ex.Message);
            }
        }
    }

}





