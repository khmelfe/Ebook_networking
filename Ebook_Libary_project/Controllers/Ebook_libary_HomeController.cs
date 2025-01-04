using EbookLibraryProject.Models;
using Ebook_Library_Project;
using System.Collections.Generic;
using System.Web.Mvc;
using Ebook_Libary_project.Models;
using System.Data.SqlClient;
using System;
using System.Linq;

namespace Ebook_Libary_project.Controllers
{
    public class Ebook_libary_HomeController : Controller
    {

        public ActionResult Ebook_home()
        {
            int userId = UserSession.GetCurrentUserId();

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

        public ActionResult SiteReview()
        {
            return View("~/Views/User/Webreview.cshtml");
        }
        [HttpPost]
        public JsonResult addreviewweb(string username,string text)
        {

            try
            {
                Userdatabase.AddReviewWeb(username,text);
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
                return Json(new { success = true, message = "Book returned successfully." });
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
                    Category=book.Category,
                    BuyingPrice = book.Buyingprice.ToString("C"),
                    BorrowPrice = book.BorrowPrice.ToString("C"),
                    minage=book.minage,
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
                allb = allb.Where(book => book.Category == genre).ToList();
            }

            // Order by buying or borrow price
            switch (priceOrder)
            {
                case "buy-low-to-high":
                    allb = allb.OrderBy(book => book.BuyingPrice).ToList();
                    break;
                case "buy-high-to-low":
                    allb = allb.OrderByDescending(book => book.BuyingPrice).ToList();
                    break;
                case "borrow-low-to-high":
                    allb = allb.OrderBy(book => book.BorrowPrice).ToList();
                    break;
                case "borrow-high-to-low":
                    allb = allb.OrderByDescending(book => book.BorrowPrice).ToList();
                    break;
            }

            return Json(allb);
        }



    }
}