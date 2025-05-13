using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Ebook_Libary_project.Models;
using Ebook_Library_Project;
using EbookLibraryProject.Models;

namespace Ebook_Libary_project.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Submit(string username, string password)
        {
            try
            {
                username = username?.Trim();
                password = password?.Trim();

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    return Json(new { success = false, message = "Email and password are required." });

                if (!IsValidEmail(username))
                    return Json(new { success = false, message = "Invalid email format." });

                string hashedInputPassword = Userdatabase.HashPassword(password);
                string hashedDbPassword = Userdatabase.GetPasswordByEmail(username)?.Trim();

                if (!string.IsNullOrEmpty(hashedDbPassword) && hashedInputPassword == hashedDbPassword)
                {
                    int userId = Userdatabase.GetUserIdByEmail(username);
                    bool isAdmin = Userdatabase.IsUser_admin(userId);
                    return Json(new { success = true, userId, isAdmin });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid email or password." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred during login: " + ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Checking_info_register(string name, string password, string con_pass, string mail, string age, bool isadmin)
        {
            Debug.WriteLine("Checking_info_register called");
            try
            {
                if (!IsValidEmail(mail))
                    return Json(new { success = false, message = "Invalid email format." });

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(con_pass))
                    return Json(new { success = false, message = "All fields are required." });

                password = password.Trim();
                con_pass = con_pass.Trim();

                bool isValid = RegistrationModel.ValitdateUser(name, password, con_pass, mail, age, isadmin);
                if (!int.TryParse(age, out int ageValue))
                    return Json(new { success = false, message = "Invalid age." });

                if (isValid)
                {
                    Userdatabase.AddUser(name.Trim(), mail.Trim(), password, ageValue, isadmin);

                    var emailService = new EmailService();
                    string subject = "Welcome to Your App!";
                    string body = $"Hi {name},<br><br>Thank you for registering at Your App!";
                    emailService.SendEmail(mail, subject, body);

                    return Json(new { success = true, message = "User was created." });
                }

                return Json(new { success = false, message = "Validation failed." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

    


      [HttpPost]
        public JsonResult email_pass_reset(string email, string username)
        {
            try
            {
                if (!IsValidEmail(email))
                {
                    return Json(new { success = false, message = "Invalid email format." });
                }

                var port = Request.Url.Port;
                Userdatabase.Sendemail_for_resetpass(email, username, port);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public ActionResult Resetpassword()
        {
            return View("~/Views/Login/Resetpassword.cshtml");
        }

        [HttpPost]
        public JsonResult resetpassword_for_user(string password, string conpassword, string username)
        {
            try
            {
                bool pass = RegistrationModel.IsPasswordMatch(password, conpassword);
                bool username_check = Userdatabase.userexistbyname(username);
                if (pass && username_check)
                {
                    bool change = Userdatabase.UpdatePasswordByUsername(username, password);
                    return Json(new { success = change });
                }
                else
                {
                    return Json(new { success = false });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        public void RunAfterPasswordChange()
        {
            Ebook_libary_HomeController controller = new Ebook_libary_HomeController();
            controller.Ebook_home();
        }

        [HttpGet]
        public ActionResult status()
        {
            int currentuser = UserSession.GetCurrentUserId();
            if (currentuser != 0)
            {
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
        }

        // Helper to check if an email is valid
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Use simple regex
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
