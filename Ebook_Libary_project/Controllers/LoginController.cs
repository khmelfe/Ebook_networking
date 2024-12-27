using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using Ebook_Libary_project.Models;
using Ebook_Library_Project;

using EbookLibraryProject.Models;
using Microsoft.Ajax.Utilities;

namespace Ebook_Libary_project.Controllers
{
    public class LoginController : Controller
    {
        public static int currentuser;
        public static Cookie CurrentCookie;
        public ActionResult Login()
        {
            return View(); // Returns the login view to show the form
        }

        [HttpPost]
        public JsonResult Submit(string username, string password,Cookie cookie=null )
        {
           
            // Simple validation for username and password (you can extend it)
            if (Ebook_Library_Project.Userdatabase.userexist(username, password))
            {
                Ebook_Library_Project.Userdatabase.IsUser_admin(6);
                //CurrentCookie = cookie;
                //Userdatabase.GetUser_details(username, password);

                //var emailService = new EmailService();
                //string subject = "Welcome to Your App!";
                //string body = $"Hi {Usermodel.Name},<br><br>Thank you for registering at Your App!";
                //emailService.SendEmail(Usermodel.Mail, subject, body);
                //currentuser = Usermodel.Id;//gets the current id.
                return Json(new { success = true, isadmin = true });
            }
            else
            {
                return Json(new { success = false, isadmin = false });
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
                    return Json(new { success = true, message = "User was Created." });
                }
                return Json(new { success = false });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }

        }
     
       
    }
}





