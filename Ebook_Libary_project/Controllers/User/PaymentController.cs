using System;
using System.Web.Mvc;

namespace Ebook_Libary_project.Controllers.User
{
    public class PaymentController : Controller
    {
        [HttpGet]
        public ActionResult Paymentvi()
        {
            
             return View("~/Views/User/PaymentView.cshtml");
        }

       
       
    }
}
