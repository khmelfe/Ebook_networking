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





