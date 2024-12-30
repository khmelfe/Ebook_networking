using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Ebook_Libary_project.Models;
using Ebook_Library_Project;
using EbookLibraryProject.Models;

namespace Ebook_Libary_project.Controllers
{
    public class LoginController : Controller
    {

        public ActionResult Login()
        {
            return View(); // Returns the login view to show the form
        }

        [HttpPost]
        public JsonResult Submit(string username, string password)
        {
            try
            {
                if (Ebook_Library_Project.Userdatabase.userexist(username, password))
                {
                    int userId = Userdatabase.GetUser_details(username, password);
                    Debug.WriteLine("Logged in successfully with user ID: " + userId);
                    bool isAdmin = Userdatabase.IsUser_admin(userId);

                    return Json(new { success = true, userId, isAdmin });

                }
                else
                {
                    return Json(new { success = false, message = "Invalid username or password." });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error during login: " + ex.Message);
                return Json(new { success = false, message = "An error occurred during login. Please try again." });
            }
        }

        [HttpPost]
        public JsonResult Checking_info_register(string name, string password, string con_pass, string mail, string age, bool isadmin)
        {
            Debug.WriteLine("Getout");
            try
            {
                
                bool Valid = RegistrationModel.ValitdateUser(name, password, con_pass, mail, age, isadmin);
                int age_dig = int.Parse(age);
                if (Valid)
                {
                    Ebook_Library_Project.Userdatabase.AddUser(name, mail, password, age_dig, isadmin);
                    // Optional: Send a welcome email (if needed)
                    var emailService = new EmailService();
                    string subject = "Welcome to Your App!";
                    string email = mail;
                    string body = $"Hi {(name)},<br><br>Thank you for registering at Your App!";
                    emailService.SendEmail(email, subject, body);
                    return Json(new { success = true, message = "User was Created." });
                }
                return Json(new { success = false });
            }
            catch (Exception ex) {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }

        } 
    }

}





