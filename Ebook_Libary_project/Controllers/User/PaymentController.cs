using System;
using System.Web.Mvc;

namespace Ebook_Libary_project.Controllers.User
{
    public class PaymentController : Controller
    {
        [HttpGet]
        public ActionResult Paymentvi()
        {
            var cart = Ebook_Libary_project.Models.Cart.GetCart();
            ViewBag.Cart = cart;
            return View("~/Views/User/PaymentView.cshtml");
        }

       
       
    }
}
