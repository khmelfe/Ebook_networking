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

        public ActionResult Ebook_home()
        {
            return View("EbooK_home");
        }

    }
}
