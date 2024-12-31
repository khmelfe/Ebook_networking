using Ebook_Libary_project.Models;
using System.Diagnostics;
using System.Web.Mvc;

namespace Ebook_Libary_project.Controllers.user
{
    public class CartController : Controller
    {
        // GET: Cart
        public ActionResult CartView()
        {
            var cart = Cart.GetCart(); // Access the shared cart
            ViewBag.Cart = cart;
            
            return View("~/Views/User/CartView.cshtml");
        }

        // POST: Remove an item from the cart
        [HttpPost]


        public ActionResult RemoveFromCart(int bookId)
        {
            var cart = Cart.GetCart(); // Access the shared cart
            cart.RemoveBookFromCart(bookId);
            return RedirectToAction("CartView");
        }
    }
}


