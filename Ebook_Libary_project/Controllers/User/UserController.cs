using Ebook_Libary_project.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EbookLibraryProject.Models;

using System.Diagnostics;
using Microsoft.Ajax.Utilities;
using Ebook_Library_Project;
using System;


namespace Ebook_Libary_project.Controllers.user
{
    public class UserController : Controller
    {




        // Static example user

        

        BookModel book;

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
        public ActionResult AddToCart(int bookId, string action, string format, bool? joinWaitingList = null)
        {
            Debug.WriteLine("add to cart start.1");

            if (action == "buy" || action == "borrow")
            {
                if (book == null || book.Id != bookId)
                {
                    book = Userdatabase.GetBookById(bookId);
                    Debug.WriteLine("2.");
                }

                var cart = Cart.GetCart();

                if (cart.Items.ContainsKey(bookId))
                {
                    Debug.WriteLine("Book is already in the cart.3");
                    return Json(new { success = false, message = "Book is already in your cart!" });
                }

                if (Userdatabase.CheckIfExistsInBorrowedBooks(UserSession.GetCurrentUserId(), bookId) ||
                    Userdatabase.CheckIfExistsInBoughtBooks(UserSession.GetCurrentUserId(), bookId) ||
                    Userdatabase.CheckIfExistsInWaitingList(UserSession.GetCurrentUserId(), bookId))
                {
                    Debug.WriteLine("4");
                    return Json(new { success = false, message = "Book already exists in library!" });
                }

                if (action == "borrow" && Userdatabase.numborrowed(UserSession.GetCurrentUserId()) >= 3)
                {
                    Debug.WriteLine("5");
                    return Json(new { success = false, message = "Already borrowed three books!" });
                }

                if (action == "borrow" && book.AvailableCopies == 0)
                {
                    if (joinWaitingList == null)
                    {
                        int waitingListLength = Userdatabase.GetWaitingListLength(bookId);
                        Debug.WriteLine("whatttt1??");
                        return Json(new { success = false, message = $"No copies available. The waiting list has {waitingListLength} people. Do you want to join the waiting list?", waitingListLength });
                    }

                    if (joinWaitingList == false)
                    {
                        Debug.WriteLine("whatttt2??");
                        return Json(new { success = false, message = "You chose not to join the waiting list." });
                    }

                    action = "waitinglist";
                }
                
                decimal price = action == "buy"
                    ? book.BuyingPrice * ((100m - book.Sale) / 100m)
                    : book.BorrowPrice * ((100m - book.Sale) / 100m);

                cart.AddBookToCart(bookId, action, price, format);
                Debug.WriteLine($"Book added to cart successfully. Action: {action}");

                return Json(new { success = true, message = "Book added to cart successfully!" });
            }

            return Json(new { success = false, message = "Invalid action." });
        }

        [HttpPost]
        public ActionResult Buynow(int bookId, string action, string format, bool? joinWaitingList = null)
        {
            Debug.WriteLine($"Buynow called with action: {action}, format: {format}");

            // Call AddToCart and process its result
            var addToCartResult = AddToCart(bookId, action, format, joinWaitingList) as JsonResult;

            if (addToCartResult != null)
            {
                dynamic resultData = addToCartResult.Data; // Extract the data from the JsonResult

                if (resultData.success == true)
                {
                    Debug.WriteLine($"AddToCart succeeded for action: {action}");
                    // Redirect to Payment route if AddToCart succeeded
                    return RedirectToRoute("Paymentname");
                }
                else if (resultData.waitingListLength != null)
                {
                    Debug.WriteLine($"AddToCart returned a waiting list message: {resultData.message}");
                    // Return JSON response for waiting list
                    return Json(new { success = false, message = resultData.message, waitingListLength = resultData.waitingListLength });
                }
                else
                {
                    Debug.WriteLine($"AddToCart failed: {resultData.message}");
                    // Handle general failure case
                    return Json(new { success = false, message = resultData.message });
                }
            }

            Debug.WriteLine("Unexpected error in Buynow.");
            // Fallback for unexpected cases
            return Json(new { success = false, message = "An unexpected error occurred while processing your request." });
        }


        public ActionResult Dashboard()
        {
            //need to add restrictions to admin
            return View("Dashboard");
        }


        [HttpPost]
        public JsonResult AddReview(int bookid, string review)
        {
            try
            {
                int userid = UserSession.GetCurrentUserId(); // Get current user ID

                if (Userdatabase.ReviewExists(userid, bookid))
                {
                    return Json(new { success = false, message = "You have already reviewed this book." });
                }

                Userdatabase.AddReview(bookid, userid, review);
                return Json(new { success = true, message = "Review added successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpGet]
        public JsonResult GetReviews(int bookid)
        {
            try
            {
                var reviews = Userdatabase.GetReviewsById(bookid); // Method to fetch reviews
                return Json(new { success = true, reviews }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public JsonResult GetWaitingListLength(int bookId)
        {
            try
            {
                int waitingListLength = Userdatabase.GetWaitingListLength(bookId); // Method to get the length of the waiting list
                return Json(new { success = true, length = waitingListLength }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}
