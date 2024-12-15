using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Ebook_Libary_project.Models;

namespace Ebook_Libary_project.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        //public ActionResult Login()
        //{
        //    // Return the partial view for AJAX requests
        //    if (Request.IsAjaxRequest())
        //    {
        //        return PartialView("_LoginPartial");
        //    }

        //    // Return the full view if it's not an AJAX request
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Submit(string username, string password)
        //{
        //    //Need to connect to database and make modifications to this code.
        //    if (ModelState.IsValid)
        //    {

        //        if (username == "fel" && password == "123")
        //        {
        //            // User exists, proceed with login (you could set session or cookies here)
        //            TempData["SuccessMessage"] = "Login successful!";
        //            return RedirectToAction("Index", "Home"); // Redirect to home page (or wherever you want)
        //        }
        //        else
        //        {
        //            // User doesn't exist, show an error message
        //            TempData["ErrorMessage"] = "Invalid username or password.";
        //            return RedirectToAction("Login"); // Redirect back to the login page
        //        }

        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Please fill in both username and password.";
        //        return RedirectToAction("Login"); // Redirect back to the login page
        //    }
        //}
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
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false });
            }
        }
    }

    }





