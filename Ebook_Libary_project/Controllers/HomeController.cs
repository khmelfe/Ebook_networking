using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ebook_Library_Project; // <-- Correct project namespace

namespace Ebook_Library_Project.Controllers // <-- Correct spelling
{
    public class HomeController : Controller
    {
        public ActionResult FillDatabase()
        {
            creditcard.FillDatabase(); // This triggers your file
            return Content("Database filled successfully!");
        }
    }
}
