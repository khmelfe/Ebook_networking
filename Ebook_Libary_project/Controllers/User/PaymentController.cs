using Ebook_Libary_project.Models;
using Ebook_Library_Project;
using EbookLibraryProject.Models;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace Ebook_Libary_project.Controllers
{
    public class PaymentController : Controller
    {
        [HttpGet]
        public ActionResult Paymentvi()
        {
            var cart = Cart.GetCart();

            foreach (var item in cart.Items)
            {
                int bookId = item.Key;
                string action = item.Value.Action;

                if (action == "borrow")
                {
                    Userdatabase.BorrowBook(Usermodel.Id, bookId);
                }
                else if (action == "buy")
                {
                    Debug.WriteLine("Book purchased successfully.");
                    Userdatabase.BuyBook(bookId, Usermodel.Id);
                }
                else
                {
                    Userdatabase.AddToWaitingList(Usermodel.Id, bookId);
                }
            }

            ViewBag.Cart = cart;
            return View("~/Views/User/PaymentView.cshtml");
        }
    }
}
