using Ebook_Libary_project.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EbookLibraryProject.Models;

using System.Diagnostics;
using Microsoft.Ajax.Utilities;
using Ebook_Library_Project;


namespace Ebook_Libary_project.Controllers.user
{
    public class UserController : Controller
    {
       

      

        // Static example user
        static UserController ()
        {
            Usermodel.Initialize(1, "John Doe", "john.doe@mail.com", "securePass123", 25, false);
        }

        Book book;

        // View for individual book page
        public ActionResult BookPage(int id)
        {

            book = Userdatabase.GetBookById(id);
            if (book == null)
                return HttpNotFound(); // Handle book not found

            return View(book);
        }

        // Action to add a book to the cart
       

        [HttpPost]
        public ActionResult AddToCart(int bookId, string action, string format)
        {
            Debug.WriteLine("add to cart start.1");
            if (action == "buy" || action == "borrow")
            {
                // Ensure the class-level book variable matches the bookId
                if (book == null || book.Id != bookId)
                {
                    book = Userdatabase.GetBookById(bookId);
                    Debug.WriteLine("2.");
                }

                var cart = Cart.GetCart(); // Access the shared cart

                // Check if the book is already in the cart
                if (cart.Items.ContainsKey(bookId))
                {
                    Debug.WriteLine("Book is already in the cart.3");
                    return Json(new { message = "Book is already in your cart!" });
                }

                // Check if the book already exists in borrowed, bought, or waiting list
                if (Userdatabase.CheckIfExistsInBorrowedBooks(Usermodel.Id, bookId) ||
                    Userdatabase.CheckIfExistsInBoughtBooks(Usermodel.Id, bookId) ||
                    Userdatabase.CheckIfExistsInWaitingList(Usermodel.Id, bookId))
                {
                    Debug.WriteLine("4");
                    return Json(new { message = "Book already exists in library!" });
                }

                // Check if the user has reached the borrow limit
                if (action == "borrow" && Userdatabase.numborrowed(Usermodel.Id) >= 3)
                {
                    Debug.WriteLine("5");
                    return Json(new { message = "Already borrowed three books!" });
                }

                // Handle unavailable books
                if (action == "borrow" && book.AvailableCopies == 0)
                {
                    action = "waitinglist";
                }

                // Add the book to the cart
                decimal price = action == "buy" ? book.BuyingPrice : book.BorrowPrice;
                cart.AddBookToCart(bookId, action, price, format);
                Debug.WriteLine($"Book added to cart successfully. Action: {action}");
                Debug.WriteLine("Add!!");
                return Json(new { message = "Book added to cart successfully!" });
            }

            return Json(new { message = "Invalid action." });
        }

           

        public ActionResult Dashboard()
        {
            //need to add restrictions to admin
            return View("Dashboard");
        }
    }
}
