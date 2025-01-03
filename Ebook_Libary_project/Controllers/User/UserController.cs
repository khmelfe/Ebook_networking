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
            Debug.WriteLine("AddToCart start.");

            try
            {


                // Check if user is logged in
                if (UserSession.GetCurrentUserId() == 0)
                {
                    Debug.WriteLine("User is not logged in.");
                    return Json(new { success = false, message = "You need to log in to perform this action.", action = "login" });
                }
                if (action == "buy" || action == "borrow")
                {
                    if (book == null || book.Id != bookId)
                    {
                        book = Userdatabase.GetBookById(bookId);
                        Debug.WriteLine("Book retrieved from database.");
                    }

                    if (book == null)
                    {
                        return Json(new { success = false, message = "Book not found.", action = "" });
                    }
                    if (book.minage > Userdatabase.GetUserAgeById(UserSession.GetCurrentUserId()))
                    {
                        return Json(new { success = false, message = "youre too young to buy this book", action = "" });
                    }
                    var cart = Cart.GetCart();

                    // Check if the book is already in the cart
                    if (cart.Items.ContainsKey(bookId))
                    {
                        Debug.WriteLine("Book is already in the cart.");
                        return Json(new { success = false, message = "Book is already in your cart!", action = "" });
                    }

                    // Check if the book already exists in the user's library
                    if (Userdatabase.CheckIfExistsInBorrowedBooks(UserSession.GetCurrentUserId(), bookId) ||
                        Userdatabase.CheckIfExistsInBoughtBooks(UserSession.GetCurrentUserId(), bookId) ||
                        Userdatabase.CheckIfExistsInWaitingList(UserSession.GetCurrentUserId(), bookId))
                    {
                        Debug.WriteLine("Book already exists in library.");
                        return Json(new { success = false, message = "Book already exists in library!", action = "" });
                    }

                    // Check borrowing limit
                    if (action == "borrow" && Userdatabase.numborrowed(UserSession.GetCurrentUserId()) >= 3)
                    {
                        Debug.WriteLine("User has already borrowed three books.");
                        return Json(new { success = false, message = "You have already borrowed three books!", action = "" });
                    }

                    // Handle no available copies
                    if (action == "borrow" && book.AvailableCopies == 0)
                    {
                        int waitingListLength = Userdatabase.GetWaitingListLength(bookId);

                        if (joinWaitingList == null)
                        {
                            Debug.WriteLine("No copies available. Prompting for waiting list.");
                            return Json(new
                            {
                                success = false,
                                message = $"No copies available. The waiting list has {waitingListLength} people. Do you want to join the waiting list?",
                                waitingListLength,
                                action = "promptWaitingList"
                            });
                        }

                        if (joinWaitingList == false)
                        {
                            Debug.WriteLine("User declined to join the waiting list.");
                            return Json(new { success = false, message = "You chose not to join the waiting list.", action = "" });
                        }

                        // Add to waiting list
                        action = "waitinglist";

                        // return Json(new { success = true, message = "You have been added to the waiting list!", action = "waitinglist" });
                    }

                    // Calculate price
                    decimal price = action == "buy"
                        ? book.BuyingPrice * ((100m - book.Sale) / 100m)
                        : book.BorrowPrice * ((100m - book.Sale) / 100m);

                    // Add the book to the cart
                    cart.AddBookToCart(bookId, action, price, format);
                    Debug.WriteLine($"Book added to cart successfully.!! Action: {action}");

                    return Json(new { success = true, message = "Book added to cart successfully!", action });
                }

                Debug.WriteLine("Invalid action.");
                return Json(new { success = false, message = "Invalid action.", action = "" });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An error occurred: {ex.Message}");
                return Json(new { success = false, message = "An unexpected error occurred. Please try again later.", action = "" });
            }
        }


        [HttpPost]
        public ActionResult Buynow(int bookId, string action, string format, bool? joinWaitingList = null)
        {
            Debug.WriteLine($"Buynow called with action: {action}, format: {format}");

            // Check if user is logged in
            if (UserSession.GetCurrentUserId() == 0)
            {
                Debug.WriteLine("User is not logged in.");
                return Json(new { success = false, message = "You need to log in to perform this action.", action = "login" });
            }

            // Attempt to add the item to the cart
            var addToCartResult = AddToCart(bookId, action, format, joinWaitingList) as JsonResult;

            if (addToCartResult != null)
            {
                // Extract data from the JsonResult
                dynamic resultData = addToCartResult.Data;

                if (resultData.success == true)
                {
                    if (resultData.action == "waitinglist")
                    {
                        Debug.WriteLine($"User added to waiting list: {resultData.message}");
                        // Redirect to payment page even for waiting list success
                        return Json(new { success = true, message = "Redirecting to payment.", action = "payment" });
                    }

                    Debug.WriteLine($"AddToCart succeeded for action: {action}. Redirecting to payment.");
                    // Redirect to payment page for direct success
                    return Json(new { success = true, message = "Redirecting to payment.", action = "payment" });
                }
                else if (resultData.action == "promptWaitingList" && resultData.waitingListLength != null)
                {
                    Debug.WriteLine($"AddToCart returned waiting list message: {resultData.message}");
                    return Json(new { success = false, message = resultData.message, waitingListLength = resultData.waitingListLength, action = "promptWaitingList" });
                }
                else
                {
                    Debug.WriteLine($"AddToCart failed: {resultData.message}");
                    return Json(new { success = false, message = resultData.message });
                }
            }

            Debug.WriteLine("Unexpected error in Buynow.");
            return Json(new { success = false, message = "An unexpected error occurred." });
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
                                                             // Check if user is logged in
                if (UserSession.GetCurrentUserId() == 0)
                {
                    Debug.WriteLine("User is not logged in.");
                    return Json(new { success = false, message = "You need to log in to perform this action.", action = "login" });
                }

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
                var reviews = Userdatabase.GetReviewsById(UserSession.GetCurrentUserId(),bookid); // Method to fetch reviews
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
