using EbookLibraryProject.Models;
using Ebook_Library_Project;
using System.Collections.Generic;
using System.Web.Mvc;
using Ebook_Libary_project.Models;
using System.Data.SqlClient;
using System;

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
    }
}
