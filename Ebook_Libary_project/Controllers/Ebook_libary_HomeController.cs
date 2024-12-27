using EbookLibraryProject.Models;
using Ebook_Library_Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Ebook_Libary_project.Controllers
{
    public class Ebook_libary_HomeController : Controller
    {
        // GET: Ebook_libary_Home
        public ActionResult Ebook_home()
        {
            // Check and return books
            List<(int UserId, int BookId)> booksToReturn = CheckBooks();
            if (booksToReturn != null)
            {
                ReturnBooks(booksToReturn);
            }

            return View("EbooK_home");
        }

        public List<(int UserId, int BookId)> CheckBooks()
        {
            string query = "SELECT UserID, BookID, ReturnDate FROM BorrowedBooks";
            List<(int UserId, int BookId)> booksToReturn = new List<(int, int)>();

            using (SqlConnection connection = new SqlConnection(Userdatabase.connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Collect data from the reader
                        while (reader.Read())
                        {
                            int userId = (int)reader["UserID"];
                            int bookId = (int)reader["BookID"];
                            DateTime returnDate = (DateTime)reader["ReturnDate"];

                            Debug.WriteLine($"Queued for return check: UserID={userId}, BookID={bookId}, ReturnDate={returnDate}");

                            if (returnDate.Date == DateTime.Today)
                            {
                                booksToReturn.Add((userId, bookId));
                            }
                        }
                    }

                    return booksToReturn;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error while checking and returning books: {ex.Message}");
                }
            }

            return null; // Return null if there was an exception
        }

        private void ReturnBooks(List<(int UserId, int BookId)> booksToReturn)
        {
            // Process books to return
            foreach (var book in booksToReturn)
            {
                try
                {
                    string message = Userdatabase.ReturnBook(book.UserId, book.BookId);
                    Debug.WriteLine($"Return process completed for UserID: {book.UserId}, BookID: {book.BookId}. Message: {message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing return for UserID: {book.UserId}, BookID: {book.BookId}. Exception: {ex.Message}");
                }
            }
        }
    }
}
