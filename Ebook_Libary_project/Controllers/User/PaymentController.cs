using System.Web.Mvc;
using Ebook_Library_Project;
using Ebook_Libary_project.Models;

namespace Ebook_Libary_project.Controllers
{
    public class PaymentController : Controller
    {
        [HttpGet]
        public ActionResult Paymentvi()
        {
            var cart = Cart.GetCart();
            int userId = UserSession.GetCurrentUserId();

            var creditCard = Userdatabase.GetCreditCardByUserId(userId);

            ViewBag.Cart = cart;
            ViewBag.CreditCard = creditCard;

            return View("~/Views/User/PaymentView.cshtml");
        }

        [HttpPost]
        public ActionResult paymentconfirm()
        {
            var cart = Cart.GetCart();
            int userId = UserSession.GetCurrentUserId();

            // Check if credit card exists, if not - save it
            var existingCard = Userdatabase.GetCreditCardByUserId(userId);
            if (existingCard == null)
            {
                string cardNumber = Request.Form["cardNumber"];
                string expiryDate = Request.Form["expiry"];
                string cvc = Request.Form["cvv"];

                // Save the new credit card
                Userdatabase.AddCreditCard(
                    userId,
                    Userdatabase.GetUserNameById(userId), // First name from user
                    "LastName", // Dummy last name
                    "123456789", // Dummy IsraeliID
                    cardNumber,
                    expiryDate,
                    cvc
                );
            }

            // Handle payment logic
            foreach (var item in cart.Items)
            {
                int bookId = item.Key;
                string action = item.Value.Action;

                if (action == "borrow")
                {
                    Userdatabase.BorrowBook(userId, bookId);
                }
                else if (action == "buy")
                {
                    Userdatabase.BuyBook(bookId, userId);
                }
                else
                {
                    Userdatabase.AddToWaitingList(userId, bookId);
                }
            }

            var emailService = new EmailService();
            string subject = "Payment Confirmation";
            string body = $@"
            <html><body>
            <p>Dear {Userdatabase.GetUserNameById(userId)},</p>
            <p>Your payment haas been accepted successfully.</p>
            </body></html>";

            emailService.SendEmail(Userdatabase.GetUserEmailById(userId), subject, body);

            cart.Items.Clear();
            return RedirectToAction("Ebook_home", "Ebook_libary_Home");
        }
    }
}
