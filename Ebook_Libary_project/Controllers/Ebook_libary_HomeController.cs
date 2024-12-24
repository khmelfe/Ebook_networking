using EbookLibraryProject.Models;
using Ebook_Library_Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ebook_Libary_project.Controllers
{
    public class Ebook_libary_HomeController : Controller
    {
        //GET: Ebook_libary_Home
        static Ebook_libary_HomeController()
        {
            Usermodel.Initialize(1, "John Doe", "john.doe@mail.com", "securePass123", 25, false);
        }

        public ActionResult Ebook_home()
        {
            return View("EbooK_home");
        }

    }
}
