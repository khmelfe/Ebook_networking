using Ebook_Libary_project;
using Ebook_Libary_project.Models;
using EbookLibraryProject.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;



namespace Ebook_Library_Project
{ //no
    public static class Userdatabase
    {
        public static string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=User;Integrated Security=True";
        //public static string connectionString = @"Data Source=DESKTOP-UFMJ78P; Integrated Security=True; TrustServerCertificate=True;";
        public static List<int> GetBoughtBookIdsByUser(int userId)
        {
            string query = "SELECT BookID FROM BoughtBooks WHERE UserID = @UserId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var bookIds = new List<int>();
                    while (reader.Read())
                    {
                        bookIds.Add((int)reader["BookID"]);
                    }
                    return bookIds;
                }
            }
        }

        public static List<int> GetBorrowedBookIdsByUser(int userId)
        {
            string query = "SELECT BookID FROM BorrowedBooks WHERE UserID = @UserId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var bookIds = new List<int>();
                    while (reader.Read())
                    {
                        bookIds.Add((int)reader["BookID"]);
                    }
                    return bookIds;
                }
            }
        }

        public static List<int> GetAllBookIds()
        {
            string query = "SELECT Id FROM Books";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var bookIds = new List<int>();
                    while (reader.Read())
                    {
                        bookIds.Add((int)reader["Id"]);
                    }
                    return bookIds;
                }
            }
        }

        public static void BuyBook(int bookId, int userId)
        {
            string query = "INSERT INTO BoughtBooks (UserID, BookID) VALUES (@UserID, @BookID)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@BookID", bookId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    Debug.WriteLine("Book purchased successfully.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        //statictics. 
        public static int Amount_of_users()
        {
            string query = "SELECT COUNT(*) FROM Users";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count;


            }

        }
        public static int Amount_of_books()
        {
            string query = "SELECT COUNT(*) FROM Books";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count;
            }

        }
        public static int Amount_of_books_inborrow_status()
        {
            string query = "SELECT COUNT(*) FROM BorrowedBooks";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count;
            }

        }
        public static int Amount_of_books_purchased()
        {
            string query = "SELECT COUNT(*) FROM BoughtBooks";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count;
            }

        }
        public static int Amount_of_Waitinglists()
        {
            string query = "SELECT COUNT(*) FROM WaitingList";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count;
            }

        }

        // Method to borrow a book
        public static void BorrowBook(int userId, int bookId)
        {
            string query = "INSERT INTO BorrowedBooks (UserID, BookID, ReturnDate) VALUES (@UserID, @BookID, @ReturnDate)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@BookID", bookId);
                command.Parameters.AddWithValue("@ReturnDate", DateTime.Now.AddDays(30)); // month borrow period

                connection.Open();
                command.ExecuteNonQuery();
            }

            string querya = "UPDATE Books SET AvailableCopies = AvailableCopies - 1 WHERE Id = @BookID AND AvailableCopies > 0";

            // Update AvailableCopies
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(querya, connection);
                command.Parameters.AddWithValue("@BookID", bookId);

                connection.Open(); // Open the connection before executing the command
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    // No rows affected means no copies are available
                    throw new Exception("No available copies of the book to borrow.");
                }
            }

        }


        public static void UpdateBookPrice(int bookId, decimal newPrice, string action)
        {
            string column = action.ToLower() == "buyprice" ? "BuyPrice" : "BorrowPrice";
            string query = "";
            if (column == "BuyPrice")
            {
                query = $"UPDATE Books SET BuyingPrice = @NewPrice WHERE Id = @BookID";
            }
            else
            {
                query = $"UPDATE Books SET BorrowPrice = @NewPrice WHERE Id = @BookID";
            }


            // Check if action is 'buying' and if newPrice is greater than the borrow price
            if (action == "BuyPrice")
            {
                // Get the current borrow price
                decimal currentBorrowPrice = GetCurrentBorrowPrice(bookId);

                // Ensure BuyPrice is greater than BorrowPrice
                if (newPrice < currentBorrowPrice)
                {
                    throw new InvalidOperationException("Buying price must be higher than borrowing price.");
                }
            }
            else
            {
                decimal currentbuyprice = GetCurrentbuyPrice(bookId);
                if (newPrice > currentbuyprice)
                {
                    throw new InvalidOperationException("Buying price must be higher than borrowing price.");
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NewPrice", newPrice);
                command.Parameters.AddWithValue("@BookID", bookId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Helper method to get the current borrow price for a book
        private static decimal GetCurrentBorrowPrice(int bookId)
        {
            decimal borrowPrice = 0;

            string query = "SELECT BorrowPrice FROM Books WHERE Id = @BookID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BookID", bookId);

                connection.Open();
                borrowPrice = (decimal)command.ExecuteScalar();
            }

            return borrowPrice;
        }
        private static decimal GetCurrentbuyPrice(int bookId)
        {
            decimal buyingPrice = 0;

            string query = "SELECT BuyingPrice FROM Books WHERE Id = @BookID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BookID", bookId);

                connection.Open();
                buyingPrice = (decimal)command.ExecuteScalar();
            }

            return buyingPrice;
        }

        // Function to change the return date for a borrowed book
        public static void UpdateReturnDate(int userId, int bookId, int days)
        {
            string query = "UPDATE BorrowedBooks SET ReturnDate = DATEADD(day, @Days, ReturnDate) WHERE UserID = @UserID AND BookID = @BookID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Days", days);
                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@BookID", bookId);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Function to remove a user from all tables and return borrowed books
        public static void RemoveUser(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Return borrowed books
                string returnBooksQuery = "DELETE FROM BorrowedBooks WHERE UserID = @UserID";

                using (SqlCommand returnBooksCommand = new SqlCommand(returnBooksQuery, connection))
                {
                    returnBooksCommand.Parameters.AddWithValue("@UserID", userId);
                    returnBooksCommand.ExecuteNonQuery();
                }

                // Remove user from BoughtBooks, WaitingList, and other tables
                string[] deleteQueries = {
                    "DELETE FROM BoughtBooks WHERE UserID = @UserID",
                    "DELETE FROM WaitingList WHERE UserID = @UserID",
                    "DELETE FROM Users WHERE Id = @UserID"
                };

                foreach (string query in deleteQueries)
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userId);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        //Function to add a book to the Books table
        //public static void AddBook(
        //     string title,
        //     string author,
        //     string category,
        //     int availableCopies,
        //     decimal buyPrice,
        //     decimal borrowPrice, int age = 0,
        //     HttpPostedFileBase imageFile = null,
        //    HttpPostedFileBase pdffile = null,
        //    HttpPostedFileBase mobifile = null,
        //    HttpPostedFileBase f2bfile = null,
        //    HttpPostedFileBase epubfile = null
        //    )
        //    {
        //        // Define directories
        //        string imageDirectory = HttpContext.Current.Server.MapPath("~/Content/Images/");
        //        string bookDirectory = HttpContext.Current.Server.MapPath("~/Content/Books/");
        //        string imagePath = null;
        //        string bookFilePath = null;


        //        // Handle the image upload
        //        if (imageFile != null && imageFile.ContentLength > 0)
        //        {
        //            if (!Directory.Exists(imageDirectory))
        //            {
        //                Directory.CreateDirectory(imageDirectory);
        //            }

        //            string imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
        //            string fullImagePath = Path.Combine(imageDirectory, imageFileName);

        //            imageFile.SaveAs(fullImagePath);
        //            imagePath = "/Content/Images/" + imageFileName; // Relative path for database
        //        }

        //        // Handle the book files formats upload

        //        if (pdffile != null && pdffile.ContentLength > 0)
        //        {
        //            if (!Directory.Exists(bookDirectory))
        //            {
        //                Directory.CreateDirectory(bookDirectory);
        //            }

        //            string pdfFileName = Guid.NewGuid().ToString() + Path.GetExtension(pdffile.FileName);
        //            string fullPdfPath = Path.Combine(bookDirectory, pdfFileName);

        //            pdffile.SaveAs(fullPdfPath);
        //            bookFilePath = "/Content/Books/" + pdfFileName; // Relative path for database
        //        }
        //        else if (mobifile != null && mobifile.ContentLength > 0)
        //        {
        //            if (!Directory.Exists(bookDirectory))
        //            {
        //                Directory.CreateDirectory(bookDirectory);
        //            }

        //            string mobiFileName = Guid.NewGuid().ToString() + Path.GetExtension(mobifile.FileName);
        //            string fullMobiPath = Path.Combine(bookDirectory, mobiFileName);

        //            mobifile.SaveAs(fullMobiPath);
        //            bookFilePath = "/Content/Books/" + mobiFileName; // Relative path for database
        //        }
        //        else if (f2bfile != null && f2bfile.ContentLength > 0)
        //        {
        //            if (!Directory.Exists(bookDirectory))
        //            {
        //                Directory.CreateDirectory(bookDirectory);
        //            }

        //            string f2bFileName = Guid.NewGuid().ToString() + Path.GetExtension(f2bfile.FileName);
        //            string fullF2BPath = Path.Combine(bookDirectory, f2bFileName);

        //            f2bfile.SaveAs(fullF2BPath);
        //            bookFilePath = "/Content/Books/" + f2bFileName; // Relative path for database
        //        }
        //        else if (epubfile != null && epubfile.ContentLength > 0)
        //        {
        //            if (!Directory.Exists(bookDirectory))
        //            {
        //                Directory.CreateDirectory(bookDirectory);
        //            }

        //            string epubFileName = Guid.NewGuid().ToString() + Path.GetExtension(epubfile.FileName);
        //            string fullEpubPath = Path.Combine(bookDirectory, epubFileName);

        //            epubfile.SaveAs(fullEpubPath);
        //            bookFilePath = "/Content/Books/" + epubFileName; // Relative path for database
        //        }

        //        // Define the SQL query
        //        string query = "INSERT INTO Books (ImagePath,BookFilePath, Name, Author, AvailableCopies, BuyingPrice, BorrowPrice,minage,Sale,category) " +
        //             "VALUES (@ImagePath, @BookFilePath, @Name, @Author, @AvailableCopies, @BuyingPrice, @BorrowPrice, @minage,0,@Category)";

        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            SqlCommand command = new SqlCommand(query, connection);

        //            // Add image path parameter
        //            command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(imagePath) ? DBNull.Value : (object)imagePath);

        //            // Add book file path parameter
        //            command.Parameters.AddWithValue("@BookFilePath", string.IsNullOrEmpty(bookFilePath) ? DBNull.Value : (object)bookFilePath);

        //            command.Parameters.AddWithValue("@Name", title);
        //            command.Parameters.AddWithValue("@Author", author);
        //            command.Parameters.AddWithValue("@Category", category);
        //            command.Parameters.AddWithValue("@AvailableCopies", availableCopies);
        //            command.Parameters.AddWithValue("@BuyingPrice", buyPrice);
        //            command.Parameters.AddWithValue("@BorrowPrice", borrowPrice);
        //            command.Parameters.AddWithValue("@minage", age);
        //            connection.Open();
        //            command.ExecuteNonQuery();
        //        }
        //    }
        public static void AddBook(
        string title,
        string author,
        string category,
        int availableCopies,
        decimal buyPrice,
        decimal borrowPrice,
        int age = 0,
        HttpPostedFileBase imageFile = null,
        HttpPostedFileBase pdffile = null,
        HttpPostedFileBase mobifile = null,
        HttpPostedFileBase f2bfile = null,
        HttpPostedFileBase epubfile = null
    )
        {
            // Define directories
            string imageDirectory = HttpContext.Current.Server.MapPath("~/Content/Images/");
            string bookDirectory = HttpContext.Current.Server.MapPath("~/Content/Books/");
            string imagePath = null;
            string pdfPath = null;
            string mobiPath = null;
            string f2bPath = null;
            string epubPath = null;

            // Handle the image upload
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                if (!Directory.Exists(imageDirectory))
                {
                    Directory.CreateDirectory(imageDirectory);
                }

                string imageFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                string fullImagePath = Path.Combine(imageDirectory, imageFileName);

                imageFile.SaveAs(fullImagePath);
                imagePath = "/Content/Images/" + imageFileName; // Relative path for database
            }

            // Handle book file uploads for each format
            if (pdffile != null && pdffile.ContentLength > 0)
            {
                string pdfFileName = Guid.NewGuid().ToString() + Path.GetExtension(pdffile.FileName);
                string fullPdfPath = Path.Combine(bookDirectory, pdfFileName);

                pdffile.SaveAs(fullPdfPath);
                pdfPath = "/Content/Books/" + pdfFileName;
            }

            if (mobifile != null && mobifile.ContentLength > 0)
            {
                string mobiFileName = Guid.NewGuid().ToString() + Path.GetExtension(mobifile.FileName);
                string fullMobiPath = Path.Combine(bookDirectory, mobiFileName);

                mobifile.SaveAs(fullMobiPath);
                mobiPath = "/Content/Books/" + mobiFileName;
            }

            if (f2bfile != null && f2bfile.ContentLength > 0)
            {
                string f2bFileName = Guid.NewGuid().ToString() + Path.GetExtension(f2bfile.FileName);
                string fullF2bPath = Path.Combine(bookDirectory, f2bFileName);

                f2bfile.SaveAs(fullF2bPath);
                f2bPath = "/Content/Books/" + f2bFileName;
            }

            if (epubfile != null && epubfile.ContentLength > 0)
            {
                string epubFileName = Guid.NewGuid().ToString() + Path.GetExtension(epubfile.FileName);
                string fullEpubPath = Path.Combine(bookDirectory, epubFileName);

                epubfile.SaveAs(fullEpubPath);
                epubPath = "/Content/Books/" + epubFileName;
            }

            // Define the SQL query
            string query = @"
        INSERT INTO Books (
            ImagePath, BookFilePathpdf, BookFilePathmobi, BookFilePathpf2b, BookFilePathepub, 
            Name, Author, AvailableCopies, BuyingPrice, BorrowPrice, minage, Sale, Category
        ) 
        VALUES (
            @ImagePath, @PdfFilePath, @MobiFilePath, @F2bFilePath, @EpubFilePath, 
            @Name, @Author, @AvailableCopies, @BuyingPrice, @BorrowPrice, @minage, 0, @Category
        )";

            // Execute the query
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                // Add parameters
                command.Parameters.AddWithValue("@ImagePath", string.IsNullOrEmpty(imagePath) ? DBNull.Value : (object)imagePath);
                command.Parameters.AddWithValue("@PdfFilePath", string.IsNullOrEmpty(pdfPath) ? DBNull.Value : (object)pdfPath);
                command.Parameters.AddWithValue("@MobiFilePath", string.IsNullOrEmpty(mobiPath) ? DBNull.Value : (object)mobiPath);
                command.Parameters.AddWithValue("@F2bFilePath", string.IsNullOrEmpty(f2bPath) ? DBNull.Value : (object)f2bPath);
                command.Parameters.AddWithValue("@EpubFilePath", string.IsNullOrEmpty(epubPath) ? DBNull.Value : (object)epubPath);
                command.Parameters.AddWithValue("@Name", title);
                command.Parameters.AddWithValue("@Author", author);
                command.Parameters.AddWithValue("@Category", category);
                command.Parameters.AddWithValue("@AvailableCopies", availableCopies);
                command.Parameters.AddWithValue("@BuyingPrice", buyPrice);
                command.Parameters.AddWithValue("@BorrowPrice", borrowPrice);
                command.Parameters.AddWithValue("@minage", age);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }



        public static void RemoveBook(int bookId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Step 1: Delete the book from BorrowedBooks if it exists
                    string deleteBorrowedBooksQuery = "DELETE FROM BorrowedBooks WHERE BookID = @BookID";
                    using (SqlCommand command = new SqlCommand(deleteBorrowedBooksQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BookID", bookId);
                        command.ExecuteNonQuery();
                    }

                    // Step 2: Delete the book from WaitingList if it exists
                    string deleteWaitingListQuery = "DELETE FROM WaitingList WHERE BookID = @BookID";
                    using (SqlCommand command = new SqlCommand(deleteWaitingListQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BookID", bookId);
                        command.ExecuteNonQuery();
                    }

                    // Step 3: Delete the book from the Books table
                    string deleteBookQuery = "DELETE FROM Books WHERE Id = @BookID";
                    using (SqlCommand command = new SqlCommand(deleteBookQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BookID", bookId);
                        command.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static List<dynamic> GetBooksBySearchTerm(string searchTerm)
        {
            string query = @"
        SELECT 
            Id, 
            Name, 
            Author, 
            Category,
            BuyingPrice,
            BorrowPrice,
            Sale,
            ImagePath,
            minage
        FROM Books 
        WHERE Name LIKE @SearchTerm OR Author LIKE @SearchTerm";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%"); // Use parameterized query
                connection.Open();

                List<dynamic> books = new List<dynamic>(); // List of anonymous objects
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Add an anonymous object with the fields you want to return
                        books.Add(new
                        {
                            Id = (int)reader["Id"],
                            Title = reader["Name"].ToString(),
                            Author = reader["Author"].ToString(),
                            Category = reader["Category"].ToString(),
                            Buyingprice = reader["BuyingPrice"],
                            BorrowPrice = reader["BorrowPrice"],
                            Sale = reader["Sale"],
                            ImagePath = reader["ImagePath"],
                            minage= reader["minage"]

                        });
                    }
                }

                return books; // Returning list of anonymous objects
            }
        }



        public static void AddToWaitingList(int userId, int bookId)
        {
            string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=User;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Get the next available queue number for the given book
                string queueQuery = "SELECT ISNULL(MAX(NumberInQueue), 0) + 1 FROM WaitingList WHERE BookID = @BookID";
                int nextQueueNumber = 1;

                using (SqlCommand queueCommand = new SqlCommand(queueQuery, connection))
                {
                    queueCommand.Parameters.AddWithValue("@BookID", bookId);
                    nextQueueNumber = (int)queueCommand.ExecuteScalar();
                }

                // Add the user to the waiting list with the calculated queue number
                string insertQuery = "INSERT INTO WaitingList (UserID, BookID, NumberInQueue) VALUES (@UserID, @BookID, @NumberInQueue)";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@UserID", userId);
                    insertCommand.Parameters.AddWithValue("@BookID", bookId);
                    insertCommand.Parameters.AddWithValue("@NumberInQueue", nextQueueNumber);

                    insertCommand.ExecuteNonQuery();
                    Debug.WriteLine("User added to the waiting list.");
                }
            }
        }


        public static void AddUser(string name, string mail, string password, int age, Boolean admin)
        {
            string queryCheck = "SELECT COUNT(*) FROM Users WHERE name = @Name";
            string query = "INSERT INTO Users (name, mail, admin, age, password) VALUES (@Name, @Mail, @Admin, @Age, @Password)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand checkCommand = new SqlCommand(queryCheck, connection))
                {
                    checkCommand.Parameters.AddWithValue("@Name", name);
                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        throw new Exception("A user with the same username already exists. Please choose a different username.");
                    }
                }
                using (SqlCommand insertCommand = new SqlCommand(query, connection))
                {
                    insertCommand.Parameters.AddWithValue("@Name", name);
                    insertCommand.Parameters.AddWithValue("@Mail", mail);
                    insertCommand.Parameters.AddWithValue("@Admin", admin);
                    insertCommand.Parameters.AddWithValue("@Age", age);
                    insertCommand.Parameters.AddWithValue("@Password", password);

                    insertCommand.ExecuteNonQuery();
                }
            }
        }
        //Reset password
        public static void Sendemail_for_resetpass(string email, string username, int port)
        {
            // Corrected query with proper parameter references
            string query = "SELECT COUNT(*) FROM Users WHERE Name = @Name AND Mail = @Mail";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                // Add the parameters with the correct names
                command.Parameters.AddWithValue("@Mail", email);
                command.Parameters.AddWithValue("@Name", username);

                connection.Open();

                // Execute the query to get the count of matching records
                int count = (int)command.ExecuteScalar();

                if (count > 0) // There is at least one matching record
                {
                    // Construct the reset URL
                    var url = $"http://localhost:{port}/Login/Resetpassword";

                    // Email service and message
                    var emailService = new EmailService();
                    string subject = "Reset Your Password";
                    string body = $@"
                            <html>
                            <body>
                                <p>Dear {username},</p>
                                <p>We received a request to reset your password. If you made this request, please click the link below to reset your password:</p>
                                <p><a href='{url}'>Reset your password</a></p>
                                <p>This link will expire in [time limit, e.g., 24 hours]. If you did not request a password reset, please ignore this email or contact our support team if you have concerns.</p>
                                <p>For your security, please do not share this email or the reset link with anyone.</p>
                                <p>Thank you,<br/>Ebook Team<br/>Support Team</p>
                            </body>
                            </html>";

                    // Send the email
                    emailService.SendEmail(email, subject, body);
                }
                else
                {
                    throw new Exception("No matching record found for the provided email and username.");
                }
            }
        }
        public static bool UpdatePasswordByUsername(string username, string newPassword)
        {

            string query = "UPDATE Users SET Password = @Password WHERE Name = @Username"; 

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Password", newPassword);
                command.Parameters.AddWithValue("@Username", username);  

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0; // Returns true if the update was successful
            }
        }
        //Make  An admin
        public static bool GrantAdminById(int userId)
        {
            string query = "UPDATE Users SET Admin = 1 WHERE Id = @UserId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();

                return rowsAffected > 0; // Returns true if the update was successful
            }
        }
        public static List<dynamic> GetUsersByUsername(string searchTerm)
        {
            string query = "SELECT Id, Name, Mail, Admin FROM Users WHERE Name LIKE @SearchTerm";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SearchTerm", "%" + searchTerm + "%"); // Use parameterized query
                connection.Open();

                List<dynamic> users = new List<dynamic>();  // List of anonymous objects
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Add an anonymous object with the fields you want to return
                        users.Add(new
                        {
                            Id = (int)reader["Id"],
                            Username = reader["Name"].ToString(),
                            Email = reader["Mail"].ToString(),
                            BorrowedBooks = 2,
                            PurchasedBooks = 2,
                            IsAdmin = (bool)reader["Admin"]
                        });
                    }
                }

                return users;  // Returning list of anonymous objects
            }
        }

        public static bool userexistbyid(int UserId)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Id =@UserID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", UserId);

                connection.Open();

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }
        public static bool userexist(string name, string password)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE name = @Name AND password = @Password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();

                int count = (int)command.ExecuteScalar();

                if (count > 0)
                {
                    // Get the user ID
                    int userId = GetUser_details(name, password);
                    Debug.WriteLine(" id" + userId);
                    // Set the userId in a cookie
                    HttpContext.Current.Response.Cookies["userId"].Value = userId.ToString();
                    HttpContext.Current.Response.Cookies["userId"].Path = "/";
                }

                return count > 0;
            }
        }
        public static bool userexistbyname(string name)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE name = @Name";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                

                connection.Open();

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }
        public static List<int> getallusersid()
        {
            string query = "SELECT Id FROM Users";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var UsersIds = new List<int>();
                    while (reader.Read())
                    {
                        UsersIds.Add((int)reader["Id"]);
                    }
                    return UsersIds;
                }
            }
        }

        public static int GetUser_details(string name, string password)
        {
            int currentId = 0;
            string query = "SELECT id FROM Users WHERE name = @Name AND password = @Password";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) // If a user is found
                    {
                        currentId = reader.GetInt32(0);
                        Debug.WriteLine(" id" + currentId);

                    }
                }
                return currentId;
            }


        }
        //public static string GetUser_name(int id) { 
        //}


        ////Is admin

        [HttpPost]
        public static bool IsUser_admin(int UserID)
        {
            string query = "SELECT Admin FROM Users WHERE Id = @UserID ";
            bool isAdmin = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", UserID);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        isAdmin = reader["Admin"] != DBNull.Value && Convert.ToBoolean(reader["Admin"]);
                        if (isAdmin)
                        {
                            return true; //user is an admin

                        }
                        else
                        {
                            return false;  //User is not an admin
                        }
                    }
                    else
                    {
                        return false;//server problems
                    }

                }
            }


        }

        // Check if a user exists in the WaitingList for a book
        public static bool CheckIfExistsInWaitingList(int userId, int bookId)
        {
            string query = "SELECT COUNT(*) FROM WaitingList WHERE UserID = @UserID AND BookID = @BookID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@BookID", bookId);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        // Check if a user has bought a specific book
        public static bool CheckIfExistsInBoughtBooks(int userId, int bookId)
        {
            string query = "SELECT COUNT(*) FROM BoughtBooks WHERE UserID = @UserID AND BookID = @BookID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@BookID", bookId);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        // Check if a user has borrowed a specific book
        public static bool CheckIfExistsInBorrowedBooks(int userId, int bookId)
        {
            string query = "SELECT COUNT(*) FROM BorrowedBooks WHERE UserID = @UserID AND BookID = @BookID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@BookID", bookId);

                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }
        public static void UpdateBookSale(int bookId, int salePercentage)
        {
            // Validate input
            if (salePercentage < 0 || salePercentage > 100)
            {
                throw new ArgumentException("Sale percentage must be between 0 and 100.");
            }



            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Update the Sale column in the database
                string updateQuery = "UPDATE Books SET Sale = @SalePercentage WHERE Id = @BookID";

                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@SalePercentage", salePercentage);
                    updateCommand.Parameters.AddWithValue("@BookID", bookId);

                    int rowsAffected = updateCommand.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        throw new Exception("Failed to update the sale percentage or book not found.");
                    }
                }
            }
        }


        public static BookModel GetBookById(int bookId)
        {
            string query = "SELECT Id, Sale, ImagePath, Name, Author,Category, BuyingPrice, BorrowPrice, AvailableCopies FROM Books WHERE Id = @BookID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BookID", bookId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new BookModel
                        {
                            Id = (int)reader["Id"],
                            Sale = (int)reader["Sale"],
                            ImagePath = reader["ImagePath"].ToString(),
                            Name = reader["Name"].ToString(),
                            Author = reader["Author"].ToString(),
                            Category = reader["Category"].ToString(),
                            BuyingPrice = (decimal)reader["BuyingPrice"],
                            BorrowPrice = (decimal)reader["BorrowPrice"],
                            AvailableCopies = (int)reader["AvailableCopies"],
                            //Age = (int)reader["Age"],
                            //File = reader["File"] != DBNull.Value ? (byte[])reader["File"] : null // Handle nullable File (varbinary)

                        };
                    }
                    else
                    {
                        return null; // Book not found
                    }
                }
            }
        }
        public static List<string> GetBookNames()
        {
            string query = "SELECT Name FROM Books";  // Query to get only book names

            List<string> bookNames = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Add each book name to the list
                        bookNames.Add(reader["Name"].ToString());
                    }
                }
            }

            return bookNames;  // Return the list of book names
        }
        public static string GetBookNamebyid(int bookid)
        {
            string name = string.Empty;
            string query = "SELECT name FROM Books WHERE id = @bookId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@bookId", bookid);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        name = reader.GetString(0);
                    }
                }
            }
            return name;
        }
        public static string GetBookpathbyid(int bookid,string format)
        {
            string path = string.Empty;
            string query = "SELECT BookFilePathpdf FROM Books WHERE id = @bookId";
            if (format=="pdf")
            query = "SELECT BookFilePathpdf FROM Books WHERE id = @bookId";
            if (format == "epub")
            query = "SELECT BookFilePathepub FROM Books WHERE id = @bookId";
            if (format == "mobi")
             query = "SELECT BookFilePathmobi FROM Books WHERE id = @bookId";
            if (format == "f2b")
             query = "SELECT BookFilePathf2b FROM Books WHERE id = @bookId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@bookId", bookid);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        path = reader.GetString(0);
                    }
                }
            }
            return path;
        }
        //get Users with borrowedbooks
        public static List<dynamic> Users_with_borrowed_books()
        {
            string query = @"
        SELECT UserID, Min(ReturnDate) AS ReturnDate
        FROM BorrowedBooks
        GROUP BY UserID"; // Group by UserID to get one entry per user, with the latest return date

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var usersWithBorrowedTime = new List<dynamic>();

                    while (reader.Read())
                    {
                        // Get the return date from the database
                        DateTime returnDate = (DateTime)reader["ReturnDate"];

                        // Calculate days left until the return date
                        int daysLeft = (returnDate - DateTime.Now).Days;

                        if (daysLeft < 0) daysLeft = 0;

                        var username = GetUserNameById((int)reader["UserID"]); // Get the username.

                        var user = new
                        {
                            UserID = (int)reader["UserID"],
                            Name = username,
                            DaysLeft = daysLeft
                        };

                        usersWithBorrowedTime.Add(user);
                    }

                    return usersWithBorrowedTime;
                }
            }
        }

        //Get user's borrowed books
        static int daysLeft = 0;
        public static List<dynamic> getusers_borrowed(int userId)
        {
            string query = "SELECT BookId, ReturnDate FROM BorrowedBooks WHERE UserId = @UserId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId); // Use the provided userId as a parameter
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var borrowedBooks = new List<dynamic>();
                    while (reader.Read())
                    {
                        DateTime returnDate = (DateTime)reader["ReturnDate"];

                        // Calculate days left until the return date
                        daysLeft = (returnDate - DateTime.Now).Days;

                        var booktitle = getbooknamebyid((int)reader["BookId"]);
                        var bookInfo = new
                        {
                            BookId = (int)reader["BookId"], // Retrieve the BookId
                            Title = booktitle,
                            ReturnDate = daysLeft // Retrieve the ReturnDate
                        };
                        borrowedBooks.Add(bookInfo);
                    }
                    return borrowedBooks;
                }
            }
        }

        public static string getbooknamebyid(int bookid)
        {
            string name = string.Empty;
            string query = "SELECT name FROM Books WHERE id = @BookId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BookId", bookid);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) // If a user is found
                    {
                        name = reader.GetString(0); // Assuming 'name' is the first column in the result
                    }
                }
            }
            return name;
        }


        //Add more time
        static DateTime newReturnDate;
        public static void update_borrowed_time(int userId, int bookId, int amountDays, string action)
        {
            string query = "SELECT ReturnDate FROM BorrowedBooks WHERE UserId = @UserId AND BookId = @BookId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@BookId", bookId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        DateTime currentReturnDate = (DateTime)reader["ReturnDate"];
                        DateTime currentdate = DateTime.Now.Date; // Today's date
                        int daysLeft = (currentReturnDate - DateTime.Now).Days;
                        // Determine the new return date based on action
                        if (action == "extend")
                        {
                            if (amountDays > 30)
                            {
                                int days_Left = (currentReturnDate - currentdate).Days;
                                int fixAmount = 30 - days_Left;
                                newReturnDate = currentReturnDate.AddDays(fixAmount);
                            }
                            else
                            {
                                newReturnDate = currentReturnDate.AddDays(amountDays - daysLeft);
                            }
                        }
                        else // "decrease" action
                        {
                            int decrese = -amountDays + daysLeft;
                            newReturnDate = currentReturnDate.AddDays(-decrese);

                            // If the new return date is before today, set it to today's date
                            if (newReturnDate <= currentdate)
                            {
                                newReturnDate = currentdate;
                            }
                        }


                    }


                }

                // Update the ReturnDate in the database
                string updateQuery = "UPDATE BorrowedBooks SET ReturnDate = @NewReturnDate WHERE UserId = @UserId AND BookId = @BookId";
                using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@NewReturnDate", newReturnDate);
                    updateCommand.Parameters.AddWithValue("@UserId", userId);
                    updateCommand.Parameters.AddWithValue("@BookId", bookId);
                    updateCommand.ExecuteNonQuery(); // Execute the update
                }
            }
        }
        ////Decrease_borrowed_time.
        //public static void Decrease_borrowed_time(int userId, int bookId, int decreaseDays)
        //{
        //    string query = "SELECT BorrowedDate, ReturnDate FROM BorrowedBooks WHERE UserId = @UserId AND BookId = @BookId";

        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        SqlCommand command = new SqlCommand(query, connection);
        //        command.Parameters.AddWithValue("@UserId", userId);
        //        command.Parameters.AddWithValue("@BookId", bookId);

        //        connection.Open();
        //        using (SqlDataReader reader = command.ExecuteReader())
        //        {
        //            if (reader.Read())
        //            {
        //                // Get the BorrowedDate and ReturnDate
        //                DateTime borrowedDate = (DateTime)reader["BorrowedDate"];
        //                DateTime returnDate = (DateTime)reader["ReturnDate"];

        //                // Calculate remaining days
        //                TimeSpan remainingTimeSpan = returnDate - DateTime.Now;
        //                int remainingDays = remainingTimeSpan.Days;

        //                // Subtract decreaseDays from the remaining time
        //                remainingDays -= decreaseDays;

        //                // If remaining days are <= 0, delete the record from the BorrowedBooks table
        //                if (remainingDays <= 0)
        //                {
        //                    // Delete the book from the BorrowedBooks table
        //                    string deleteQuery = "DELETE FROM BorrowedBooks WHERE UserId = @UserId AND BookId = @BookId";
        //                    SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
        //                    deleteCommand.Parameters.AddWithValue("@UserId", userId);
        //                    deleteCommand.Parameters.AddWithValue("@BookId", bookId);
        //                    deleteCommand.ExecuteNonQuery();
        //                    Console.WriteLine("The borrowed book record has been removed because the borrowed time has expired.");
        //                }
        //                else
        //                {
        //                    // Update the ReturnDate based on the new remaining time
        //                    DateTime newReturnDate = DateTime.Now.AddDays(remainingDays);
        //                    string updateQuery = "UPDATE BorrowedBooks SET ReturnDate = @NewReturnDate WHERE UserId = @UserId AND BookId = @BookId";
        //                    SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
        //                    updateCommand.Parameters.AddWithValue("@NewReturnDate", newReturnDate);
        //                    updateCommand.Parameters.AddWithValue("@UserId", userId);
        //                    updateCommand.Parameters.AddWithValue("@BookId", bookId);
        //                    updateCommand.ExecuteNonQuery();
        //                    Console.WriteLine($"Updated the borrowed time. New return date is: {newReturnDate.ToShortDateString()}");
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("No record found for the given UserId and BookId.");
        //            }
        //        }
        //    }
        //}


        public static int numborrowed(int userId)
        {
            int totalBooks = 0;

            string query = @"
        SELECT 
            (SELECT COUNT(*) FROM WaitingList WHERE UserID = @UserID) +
            (SELECT COUNT(*) FROM BorrowedBooks WHERE UserID = @UserID) AS TotalBooks";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userId);

                connection.Open();
                totalBooks = (int)command.ExecuteScalar();
            }

            return totalBooks;
        }


        public static string ReturnBook(int userId, int bookId)
        {
            string message = string.Empty;
            Debug.WriteLine("Started return of bookId: " + bookId);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if there is anyone in the waiting list for this book
                string waitingListQuery = @"
            SELECT TOP 1 UserID, NumberInQueue 
            FROM WaitingList 
            WHERE BookID = @BookID 
            ORDER BY NumberInQueue";

                using (SqlCommand waitingListCommand = new SqlCommand(waitingListQuery, connection))
                {
                    waitingListCommand.Parameters.AddWithValue("@BookID", bookId);

                    SqlDataReader reader = null;
                    try
                    {
                        Debug.WriteLine("Executing waiting list check for bookId: " + bookId);
                        reader = waitingListCommand.ExecuteReader();

                        if (reader.Read())
                        {
                            int nextUserId = (int)reader["UserID"];
                            int numberInQueue = (int)reader["NumberInQueue"];
                            reader.Close();

                            // Remove the current entry from BorrowedBooks
                            RemoveBorrowedBook(connection, userId, bookId);

                            // Increase available copies
                            IncreaseAvailableCopies(connection, bookId);

                            // Assign the book to the next user in line
                            BorrowBook(nextUserId, bookId);

                            // Remove the user from the waiting list
                            RemoveFromWaitingList(connection, nextUserId, bookId);

                            // Update queue numbers for remaining users
                            UpdateQueueNumbers(connection, bookId);

                            // Send notification to the next user
                            var emailService = new EmailService();
                            string subject = "Book was returned!";
                            string email = GetUserEmailById(nextUserId);
                            string body = $"Hi {GetUserNameById(nextUserId)},<br><br>The book you've been waiting for is now available!";
                            emailService.SendEmail(email, subject, body);

                            message = $"Book returned and assigned to the next user in line (UserID: {nextUserId}).";
                        }
                        else
                        {
                            reader.Close();

                            // No one in the waiting list
                            RemoveBorrowedBook(connection, userId, bookId);
                            IncreaseAvailableCopies(connection, bookId);

                            message = "Book returned successfully. Available copies increased.";
                        }
                    }
                    catch (Exception ex)
                    {
                        reader?.Close();
                        throw new Exception($"Error processing ReturnBook: {ex.Message}", ex);
                    }
                }
            }

            return message;
        }

        // Helper method to remove the book from BorrowedBooks
        private static void RemoveBorrowedBook(SqlConnection connection, int userId, int bookId)
        {
            Debug.WriteLine("remove borrowed: " + bookId+userId);
            string deleteBorrowedQuery = "DELETE FROM BorrowedBooks WHERE UserID = @UserID AND BookID = @BookID";
            using (SqlCommand deleteBorrowedCommand = new SqlCommand(deleteBorrowedQuery, connection))
            {
                deleteBorrowedCommand.Parameters.AddWithValue("@UserID", userId);
                deleteBorrowedCommand.Parameters.AddWithValue("@BookID", bookId);
                deleteBorrowedCommand.ExecuteNonQuery();
            }
        }

        // Helper method to increase available copies
        private static void IncreaseAvailableCopies(SqlConnection connection, int bookId)
        {
            Debug.WriteLine("av copies+1: " + bookId);
            string updateCopiesQuery = "UPDATE Books SET AvailableCopies = AvailableCopies + 1 WHERE Id = @BookID";
            using (SqlCommand updateCopiesCommand = new SqlCommand(updateCopiesQuery, connection))
            {
                updateCopiesCommand.Parameters.AddWithValue("@BookID", bookId);
                updateCopiesCommand.ExecuteNonQuery();
            }
        }

        // Helper method to remove a user from the waiting list
        private static void RemoveFromWaitingList(SqlConnection connection, int userId, int bookId)
        {
            Debug.WriteLine("remove waiting list " + bookId+"-"+userId);
            string deleteFromWaitingListQuery = "DELETE FROM WaitingList WHERE UserID = @UserID AND BookID = @BookID";
            using (SqlCommand deleteFromWaitingListCommand = new SqlCommand(deleteFromWaitingListQuery, connection))
            {
                deleteFromWaitingListCommand.Parameters.AddWithValue("@UserID", userId);
                deleteFromWaitingListCommand.Parameters.AddWithValue("@BookID", bookId);
                deleteFromWaitingListCommand.ExecuteNonQuery();
            }
        }

        // Helper method to update queue numbers in the waiting list
        private static void UpdateQueueNumbers(SqlConnection connection, int bookId)
        {
            Debug.WriteLine("queue " + bookId);
            string updateQueueQuery = @"
        UPDATE WaitingList
        SET NumberInQueue = NumberInQueue - 1
        WHERE BookID = @BookID AND NumberInQueue > 1";
            using (SqlCommand updateQueueCommand = new SqlCommand(updateQueueQuery, connection))
            {
                updateQueueCommand.Parameters.AddWithValue("@BookID", bookId);
                updateQueueCommand.ExecuteNonQuery();
            }
        }



        // Method to get email by user ID
        public static string GetUserEmailById(int userId)
        {
            string email = string.Empty;
            string query = "SELECT mail FROM Users WHERE id = @UserId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) // If a user is found
                    {
                        email = reader.GetString(0); // Assuming 'mail' is the first column in the result
                    }
                }
            }
            return email;
        }

        // Method to get name by user ID
        public static string GetUserNameById(int userId)
        {
            string name = string.Empty;
            string query = "SELECT name FROM Users WHERE id = @UserId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) // If a user is found
                    {
                        name = reader.GetString(0); // Assuming 'name' is the first column in the result
                    }
                }
            }
            return name;
        }

        // Method to get age by user ID
        public static int GetUserAgeById(int userId)
        {
            int age = 0;
            string query = "SELECT Age FROM Users WHERE id = @UserId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) // If a user is found
                    {
                        age = reader.GetInt32(0); // Assuming 'age' is the first column in the result
                    }
                }
            }
            return age;
        }


        public static void AddReview(int bookid, int userid, string review)
        {
            string query = "INSERT INTO Reviews (userid, bookid, review) VALUES (@UserId, @BookId, @Review)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userid);
                command.Parameters.AddWithValue("@BookId", bookid);
                command.Parameters.AddWithValue("@Review", review);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
        public static void AddReviewWeb(string Username, string review)
        {
            if (string.IsNullOrWhiteSpace(Username))
                throw new ArgumentException("Username cannot be null or empty.", nameof(Username));

            if (string.IsNullOrWhiteSpace(review))
                throw new ArgumentException("Review cannot be null or empty.", nameof(review));

                int id = Getuseridbyname(Username);
                string query = "INSERT INTO WebReviews (userid, review) VALUES (@UserId,  @Review)";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@UserId", id);
                    command.Parameters.AddWithValue("@Review", review);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
         public static List<dynamic> GetWebReviews()
        {
            List<dynamic> reviews = new List<dynamic>();

            string query = "SELECT userid, review FROM WebReviews";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reviews.Add(new
                        {
                            username = reader["userid"].ToString(),
                            ReviewText = reader["review"].ToString()
                        });
                    }
                }
            }

            return reviews;
        }
    
         public static bool check_ifmade_webreview(int userid)
        {
            string query = "SELECT COUNT(*) FROM WebReviews WHERE userid =@UserID";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userid);

                connection.Open();

                int count = (int)command.ExecuteScalar();

                return count > 0;
            }
        }

        public static int Getuseridbyname(string Username)
        {
            if (userexistbyname(Username))
            {
                string query = "SELECT id FROM Users WHERE Name = @Name";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", Username);
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read()) // If a user is found
                        {
                            return (int)reader["id"]; ;
                        }
                        else
                        {
                            throw new Exception("Username Doesn't exist");
                        }
                    }

                }

            }
            else
            {
                throw new Exception("Username Doesn't exist");
            }
        }


        public static bool ReviewExists(int userid, int bookid)
        {
            string query = "SELECT COUNT(*) FROM Reviews WHERE userid = @UserId AND bookid = @BookId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userid);
                command.Parameters.AddWithValue("@BookId", bookid);

                connection.Open();
                return (int)command.ExecuteScalar() > 0;
            }
        }
        public static int GetWaitingListLength(int bookid)
        {
            string query = "SELECT COUNT(*) FROM BorrowedBooks WHERE BookID = @bookid";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@bookid", bookid); // Properly bind the parameter
                connection.Open();
                int count = (int)command.ExecuteScalar(); // Execute the query and retrieve the count
                return count;
            }
        }
        public static int GetWaitingListLengthsize(int bookid)
        {
            string query = "SELECT COUNT(*) FROM WaitingList WHERE BookID = @bookid";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@bookid", bookid); // Properly bind the parameter
                connection.Open();
                int count = (int)command.ExecuteScalar(); // Execute the query and retrieve the count
                return count;
            }
        }
        public static List<dynamic> GetBooksInWaitingList()
        {
            string query = @"
        SELECT DISTINCT  BookID
        FROM WaitingList"; // Query to get all BookIDs from the WaitingList table

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var booksInWaitingList = new List<dynamic>();

                    while (reader.Read())
                    {
                        // Get the BookID from the database
                        int bookId = (int)reader["BookID"];
                        int length_list = GetWaitingListLengthsize(bookId);


                        string bookName = GetBookNamebyid(bookId);


                        // Create an object with the BookID and BookName
                        var book = new
                        {
                            BookID = bookId,
                            Name = bookName,
                            length = length_list

                        };

                        booksInWaitingList.Add(book);
                    }

                    return booksInWaitingList;
                }
            }
        }
        public static List<dynamic> GetUsersInWaitingList(int waitingListId)
        {
            string query = @"
    SELECT UserID, BookID, NumberInQueue
    FROM WaitingList WHERE BookID =@waitingListId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@WaitingListID", waitingListId);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var usersInWaitingList = new List<dynamic>();

                    while (reader.Read())
                    {
                        // Get the UserID and Name from the database
                        int userId = (int)reader["UserID"];
                        string userName = GetUserNameById(userId);
                        int numberinqueue = (int)reader["NumberinQueue"];

                        // Create an object with the UserID and Name
                        var user = new
                        {
                            UserID = userId,
                            Name = userName,
                            queue = numberinqueue
                        };

                        usersInWaitingList.Add(user);
                    }

                    return usersInWaitingList;
                }
            }
        }
        public static void RemoveFromWaitingListAndUpdateQueue(int bookId, int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Step 1: Remove the specified user from the waiting list
                string removeFromWaitingListQuery = "DELETE FROM WaitingList WHERE BookID = @BookID AND UserID = @UserID";

                using (SqlCommand removeCommand = new SqlCommand(removeFromWaitingListQuery, connection))
                {
                    removeCommand.Parameters.AddWithValue("@BookID", bookId);
                    removeCommand.Parameters.AddWithValue("@UserID", userId);
                    removeCommand.ExecuteNonQuery();
                }

                // Step 2: Update the queue numbers for the remaining users
                string updateQueueQuery = @"
            WITH CTE AS (
                SELECT 
                    UserID, 
                    ROW_NUMBER() OVER (ORDER BY NumberInQueue) AS NewQueueNumber
                FROM WaitingList
                WHERE BookID = @BookID
            )
            UPDATE WaitingList
            SET NumberInQueue = CTE.NewQueueNumber
            FROM WaitingList
            INNER JOIN CTE ON WaitingList.UserID = CTE.UserID AND WaitingList.BookID = @BookID";

                using (SqlCommand updateCommand = new SqlCommand(updateQueueQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@BookID", bookId);
                    updateCommand.ExecuteNonQuery();
                }
            }
        }

        public static List<Review> GetReviewsById(int userid, int bookid)
        {
            string query = "SELECT userid, bookid, review FROM Reviews WHERE userid = @UserId AND bookid = @BookId";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                // Add parameters for the user ID and book ID
                command.Parameters.AddWithValue("@UserId", userid);
                command.Parameters.AddWithValue("@BookId", bookid);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    var reviews = new List<Review>();
                    while (reader.Read())
                    {
                        reviews.Add(new Review
                        {
                            UserId = reader.GetInt32(0),
                            BookId = reader.GetInt32(1),
                            ReviewText = reader.GetString(2)
                        });
                    }
                    return reviews;
                }
            }
        }

        public static void RemoveBoughtBook(int userId, int bookId)
        {
            string query = "DELETE FROM BoughtBooks WHERE UserID = @UserId AND BookID = @BookId";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@BookId", bookId);
                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new Exception("Failed to remove the book. It might not exist in the user's bought books.");
                }
            }
        }





    }
}