﻿using EbookLibraryProject.Models;
using Ebook_Library_Project;
using System.Collections.Generic;
using System.Web.Mvc;
using Ebook_Libary_project.Models;
using System.Data.SqlClient;
using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Web;

namespace Ebook_Libary_project.Controllers
{
    public class Ebook_libary_HomeController : Controller
    {

        public ActionResult Ebook_home()
        {
            int userId = UserSession.GetCurrentUserId();
            CheckAndHandleBorrowedBooks();

            if (userId == 0)
            {
                ViewBag.Message = "Login to see books";
                ViewBag.BoughtBooks = null; // Explicitly set to null to ensure the view handles it
                ViewBag.BorrowedBooks = null;
                return View();
            }


            var boughtBooks = GetBoughtBooks(userId);
            var borrowedBooks = GetBorrowedBooksWithReturnDate(userId);

            ViewBag.BoughtBooks = boughtBooks;
            ViewBag.BorrowedBooks = borrowedBooks;

            return View();
        }
        [HttpGet]
        public ActionResult Check_webreview_ofuser()
        {
            int currentuser = UserSession.GetCurrentUserId();

            if (Userdatabase.check_ifmade_webreview(currentuser))
            {
                return Json(new { success = true, message = "You have already submitted a review." }, JsonRequestBehavior.AllowGet);
            }
            else
            {

                return Json(new { success = false, message = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SiteReview()
        {
            int userId = UserSession.GetCurrentUserId();
            string userName = Userdatabase.GetUserNameById(userId); // Replace with your actual method to fetch the name

            // Pass the user name to the view
            ViewBag.UserName = userName;
            return View("~/Views/User/Webreview.cshtml");
        }
        [HttpPost]
        public JsonResult addreviewweb(string text)
        {
            int userId = UserSession.GetCurrentUserId();
            string userName = Userdatabase.GetUserNameById(userId);
            try
            {
                Userdatabase.AddReviewWeb(userName, text);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex });
            }
        }

        public JsonResult getwebreviews()
        {
            try
            {
                List<dynamic> webreviews = Userdatabase.GetWebReviews();
                return Json(new { success = true, data = webreviews }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }




        private List<BookModel> GetBoughtBooks(int userId)
        {
            var bookIds = Userdatabase.GetBoughtBookIdsByUser(userId);
            var books = new List<BookModel>();

            foreach (var bookId in bookIds)
            {
                var book = Userdatabase.GetBookById(bookId);
                if (book != null)
                {
                    books.Add(book);
                }
            }

            return books;
        }

        private List<(BookModel Book, DateTime ReturnDate)> GetBorrowedBooksWithReturnDate(int userId)
        {
            string query = "SELECT BookID, ReturnDate FROM BorrowedBooks WHERE UserID = @UserId";
            var borrowedBooks = new List<(BookModel, DateTime)>();

            using (SqlConnection connection = new SqlConnection(Userdatabase.connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int bookId = (int)reader["BookID"];
                        DateTime returnDate = (DateTime)reader["ReturnDate"];

                        var book = Userdatabase.GetBookById(bookId);
                        if (book != null)
                        {
                            borrowedBooks.Add((book, returnDate));
                        }
                    }
                }
            }

            return borrowedBooks;
        }
        [HttpPost]
        public JsonResult RemoveBoughtBook(int bookId)
        {
            try
            {
                Userdatabase.RemoveBoughtBook(UserSession.GetCurrentUserId(), bookId);
                return Json(new { success = true, message = "Book removed successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }
        [HttpPost]
        public JsonResult ReturnBook(int bookId)
        {
            try
            {
                Userdatabase.ReturnBook(UserSession.GetCurrentUserId(), bookId);
                return Json(new { success = true, message = "Book" + bookId + " returned successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public JsonResult GetBookSuggestions(string query)
        {
            var books = Userdatabase.GetBooksBySearchTerm(query)
                .Select(book => new { Title = book.Title, Author = book.Author })
                .Take(10) // Limit suggestions to 10
                .ToList();

            return Json(books, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SearchBooks(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return Json(new { error = true, message = "Search term cannot be empty." });
            }

            var books = Userdatabase.GetBooksBySearchTerm(searchTerm)
                .Select(book => new
                {
                    Id = book.Id,
                    Name = book.Title,
                    Author = book.Author,
                    Category = book.Category,
                    BuyingPrice = book.Buyingprice.ToString("C"),
                    BorrowPrice = book.BorrowPrice.ToString("C"),
                    minage = book.minage,
                    Sale = book.Sale,
                    DiscountedPrice = book.Sale > 0
                ? (book.Buyingprice * (1 - book.Sale / 100m)).ToString("C")
                : null
                })
        .ToList();


            // Return the list of books as JSON
            return Json(books);
        } 

        [HttpPost]
        public JsonResult FilterBooks(string genre, string priceOrder)
        {
            var bookIds = Userdatabase.GetAllBookIds();
            List<BookModel> allb = new List<BookModel>();
            List<BookModel> b = new List<BookModel>();
            Debug.WriteLine("genre-", genre);
            foreach (var bookId in bookIds)
            {
                var book = Userdatabase.GetBookById(bookId);
                if (book != null)
                {
                    allb.Add(book);
                }
            }

            // Filter by genre
            if (!string.IsNullOrEmpty(genre))
            {
                foreach (var book in allb)
                {
                    Debug.WriteLine("book-" + book.Category + book.Name);


                    if (book.Category.Trim() == (genre.Trim()))
                    {
                        b.Add(book);

                        Debug.WriteLine("equals-" + book.Category + genre);
                    }
                    else
                    {
                        Debug.WriteLine(book.Category, genre);

                    }
                }

            }
            else { b = allb; }
            Debug.WriteLine(allb.Count);
            // Order by buying or borrow price
            switch (priceOrder)
            {
                case "buy-low-to-high":
                    b = b.OrderBy(book => book.BuyingPrice).ToList();
                    break;
                case "buy-high-to-low":
                    b = b.OrderByDescending(book => book.BuyingPrice).ToList();
                    break;
                case "borrow-low-to-high":
                    b = b.OrderBy(book => book.BorrowPrice).ToList();
                    break;
                case "borrow-high-to-low":
                    b = b.OrderByDescending(book => book.BorrowPrice).ToList();
                    break;
            }

            return Json(b);
        }

        [HttpGet]
        public ActionResult GetBookFilePath(int bookId, string format)
        {
            try
            {
                string relativePath = Userdatabase.GetBookpathbyid(bookId, format);
                string filePath = Server.MapPath("~" + relativePath);


                if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                {
                    return new HttpStatusCodeResult(404, "File not found or invalid format.");
                }

                string mimeType = MimeMapping.GetMimeMapping(filePath);

                return File(filePath, mimeType, Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, $"Error occurred: {ex.Message}");
            }
        }

        public ActionResult ViewBookPdf(int bookId)
        {
            try
            {
                string relativePath = Userdatabase.GetBookpathbyid(bookId, "pdf");
                string filePath = Server.MapPath("~" + relativePath);

                if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                {
                    return HttpNotFound("PDF file not found.");
                }

                string pdfUrl = Url.Content("~" + relativePath); 
                return View("BookViewer", model: pdfUrl);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, $"Error occurred: {ex.Message}");
            }
        }


        public void CheckAndHandleBorrowedBooks()
        {
            Debug.WriteLine("Started processing borrowed books...");
            try
            {
                string query = "SELECT UserID, BookID, ReturnDate, Emailsent FROM BorrowedBooks";
                using (SqlConnection connection = new SqlConnection(Userdatabase.connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int userId = (int)reader["UserID"];
                            int bookId = (int)reader["BookID"];
                            DateTime returnDate = (DateTime)reader["ReturnDate"];
                            int emailSent = reader["Emailsent"] != DBNull.Value ? (int)reader["Emailsent"] : 0;
                            DateTime today = DateTime.Today;

                            Debug.WriteLine($"{returnDate} - {today} = {(returnDate - today).Days}");

                            if (returnDate.Date == today)
                            {
                                Userdatabase.ReturnBook(userId, bookId);
                                Debug.WriteLine($"Book ID {bookId} returned for user ID {userId}.");
                            }
                            else if ((returnDate - today).Days <= 5 && emailSent == 0)
                            {
                                // Send a warning email if the book is due in 5 days and email hasn't been sent
                                string email = Userdatabase.GetUserEmailById(userId);
                                string userName = Userdatabase.GetUserNameById(userId);
                                var emailService = new EmailService();
                                string subject = "Reminder: Book Return Due in 5 Days";
                                string body = $@"
                            Dear {userName},

                            This is a friendly reminder that the book you borrowed {Userdatabase.getbooknamebyid(bookId)}) is due to be returned in 5 days.
                            Please make arrangements to return the book on time to avoid late fees.

                            Thank you,
                            Ebook Library Team";

                                emailService.SendEmail(email, subject, body);
                                Debug.WriteLine($"Reminder email sent to {userName}.");

                                UpdateEmailSentStatus(bookId, userId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CheckAndHandleBorrowedBooks: {ex.Message}");
            }
        }

        private void UpdateEmailSentStatus(int bookId, int userId)
        {
            try
            {
                string query = "UPDATE BorrowedBooks SET Emailsent = 1 WHERE BookID = @BookID AND UserID = @UserID";
                using (SqlConnection connection = new SqlConnection(Userdatabase.connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@BookID", bookId);
                    command.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    Debug.WriteLine(rowsAffected > 0
                        ? $"Emailsent updated for Book ID {bookId}, User ID {userId}."
                        : $"No update made for Book ID {bookId}, User ID {userId}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Emailsent status: {ex.Message}");
            }
        }




    }
}
    
