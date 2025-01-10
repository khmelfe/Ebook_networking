using Ebook_Libary_project.Models;
using Ebook_Library_Project;
using EbookLibraryProject.Models;
using System;
using System.Diagnostics;
using System.Security.Policy;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Ebook_Libary_project.Controllers
{
    public class PaymentController : Controller
    {
        [HttpGet]
        public ActionResult Paymentvi()
        {
            var cart = Cart.GetCart();
          
            ViewBag.Cart = cart;
            return View("~/Views/User/PaymentView.cshtml");
        }

        [HttpPost]
        public ActionResult paymentconfirm()
        {
            var cart = Cart.GetCart();
            Debug.WriteLine("paying.");
            foreach (var item in cart.Items)
            {
                int bookId = item.Key;
                string action = item.Value.Action;

                if (action == "borrow")
                {
                    Debug.WriteLine("Book borrowed successfully.");
                    Userdatabase.BorrowBook(UserSession.GetCurrentUserId(), bookId);
                }
                else if (action == "buy")
                {
                    Debug.WriteLine("Book purchased successfully.");
                    Userdatabase.BuyBook(bookId, UserSession.GetCurrentUserId());
                }
                else
                {
                    Userdatabase.AddToWaitingList(UserSession.GetCurrentUserId(), bookId);
                }
            }
            var emailService = new EmailService();
            string subject = "Reset Your Password";
            string body = $@"
        <html>
        <body>
            <p>Dear {Userdatabase.GetUserNameById(UserSession.GetCurrentUserId())},</p>
            <p>payment exepted</p>
          
        </body>
        </html>";

            // Send the email
            emailService.SendEmail(Userdatabase.GetUserEmailById(UserSession.GetCurrentUserId()), subject, body);
            cart.Items.Clear();
            return RedirectToAction("Ebook_home", "Ebook_libary_Home");
        }
    }
}
