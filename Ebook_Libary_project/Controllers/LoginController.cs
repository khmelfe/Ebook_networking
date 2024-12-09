//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Ebook_Libary_project.Models;

//namespace Ebook_Libary_project.Controllers
//{
//    public class LoginController : Controller
//    {
//        // GET: Login
//        public ActionResult Login()
//        {
//            ViewData["UseLayout"] = false;
//            return View(new UserModel());
//        }

//        [HttpPost]
//        public ActionResult Submit(UserModel USER)
//        {
//            if (ModelState.IsValid)
//            {
//                TempData["ErrorMessage"] = "Great job";
//                ViewData["UseLayout"] = false;
//                return View("Login", USER); // Renders the Login view with the message
//            }
//            else
//            {
//                TempData["ErrorMessage"] = "Well, try again m8";
//                ViewData["UseLayout"] = false;
//                return View("Login", USER); // Renders the Login view with the message
//            }
//        }

//    }
//}

    
    
    