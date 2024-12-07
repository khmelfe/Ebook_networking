using Ebook_Libary_project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Ebook_Libary_project.Controllers.user
{
    public class UserController : Controller
    {
        List<Book> books = BookDatabase.Books;

        // View for individual book page
        public ActionResult BookPage(int id)
        {
            var book = books.FirstOrDefault(b => b.Id == id);
            if (book == null)
                return HttpNotFound(); // Handle book not found

            return View(book);
        }

        // Action to add a book to the cart
        [HttpPost]
        public ActionResult AddToCart(int bookId, string action, string format)
        {
            if (action == "buy" || action == "borrow")
            {
                var book = books.FirstOrDefault(b => b.Id == bookId);
                if (book == null)
                    return HttpNotFound(); // Handle book not found

                decimal price = action == "buy" ? book.BuyingPrice : book.BorrowPrice;

                var cart = Cart.GetCart(); // Access the shared cart
                if (!cart.Items.ContainsKey(bookId)) // If the book isn't already in the cart
                {
                    cart.AddBookToCart(bookId, action, price, format);
                }

                return Json(new { success = true, message = "Book added to cart successfully!" });
            }

            return Json(new { success = false, message = "Invalid action." });
        }
    }
}
