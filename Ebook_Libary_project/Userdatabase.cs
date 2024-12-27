using Ebook_Libary_project;
using Ebook_Libary_project.Models;
using EbookLibraryProject.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Web;

namespace Ebook_Library_Project
{
    public static class Userdatabase
    {
        public static string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=User;Integrated Security=True";
       // private static string connectionString = @"Data Source=DESKTOP-UFMJ78P; Integrated Security=True; TrustServerCertificate=True;";
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

       

        // Method to borrow a book
        public static void BorrowBook(int userId, int bookId)
        {
            string query = "INSERT INTO BorrowedBooks (UserID, BookID, ReturnDate) VALUES (@UserID, @BookID, @ReturnDate)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@BookID", bookId);
                command.Parameters.AddWithValue("@ReturnDate", DateTime.Now.AddDays(14)); // Example: 2-week borrow period

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Function to change the price for buying or borrowing a book
        public static void UpdateBookPrice(int bookId, decimal newPrice, string action)
        {
            string column = action.ToLower() == "buying" ? "BuyPrice" : "BorrowPrice";
            string query = $"UPDATE Books SET {column} = @NewPrice WHERE Id = @BookID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NewPrice", newPrice);
                command.Parameters.AddWithValue("@BookID", bookId);

                connection.Open();
                command.ExecuteNonQuery();
            }
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

        // Function to add a book to the Books table
        public static void AddBook(string title, string author, int availableCopies, decimal buyPrice, decimal borrowPrice, string image)
        {
            string query = "INSERT INTO Books (ImagePath, Name, Author, AvailableCopies, BuyingPrice, BorrowPrice, Sale) VALUES (@ImagePath, @Name, @Author, @AvailableCopies, @BuyingPrice, @BorrowPrice, 0)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                // If the image path is null or empty, set the parameter value to DBNull
                if (string.IsNullOrEmpty(image))
                {
                    command.Parameters.AddWithValue("@ImagePath", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@ImagePath", image);
                }

                command.Parameters.AddWithValue("@Name", title);
                command.Parameters.AddWithValue("@Author", author);
                command.Parameters.AddWithValue("@AvailableCopies", availableCopies);
                command.Parameters.AddWithValue("@BuyingPrice", buyPrice);
                command.Parameters.AddWithValue("@BorrowPrice", borrowPrice);

                connection.Open();
                command.ExecuteNonQuery();
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


        public static void AddUser(string name,string mail,  string password, int age, Boolean admin)
        {
            string queryCheck = "SELECT COUNT(*) FROM Users WHERE name = @Name";
            string query = "INSERT INTO Users (name, mail, admin, age, password) VALUES (@Name, @Mail, @Admin, @Age, @Password)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using(SqlCommand checkCommand = new SqlCommand(queryCheck, connection))
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


        public static int GetUser_details(string name, string password){
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

        public static Book GetBookById(int bookId)
        {
            string query = "SELECT Id, Sale, ImagePath, Name, Author, BuyingPrice, BorrowPrice, AvailableCopies FROM Books WHERE Id = @BookID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@BookID", bookId);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Book
                        {
                            Id = (int)reader["Id"],
                            Sale = (int)reader["Sale"],
                            ImagePath = reader["ImagePath"].ToString(),
                            Name = reader["Name"].ToString(),
                            Author = reader["Author"].ToString(),
                            BuyingPrice = (decimal)reader["BuyingPrice"],
                            BorrowPrice = (decimal)reader["BorrowPrice"],
                            AvailableCopies = (int)reader["AvailableCopies"]
                        };
                    }
                    else
                    {
                        return null; // Book not found
                    }
                }
            }
        }

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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if there is anyone in the waiting list for this book
                string waitingListQuery = "SELECT TOP 1 UserID, NumberInQueue FROM WaitingList WHERE BookID = @BookID ORDER BY NumberInQueue";

                using (SqlCommand waitingListCommand = new SqlCommand(waitingListQuery, connection))
                {
                    waitingListCommand.Parameters.AddWithValue("@BookID", bookId);

                    // Explicitly close any lingering readers before executing
                    SqlDataReader reader = null;
                    try
                    {
                        reader = waitingListCommand.ExecuteReader();
                        if (reader.Read())
                        {
                            int nextUserId = (int)reader["UserID"];
                            int numberInQueue = (int)reader["NumberInQueue"];

                            // Close the reader before executing the next command
                            reader.Close();

                            // Remove the current entry from BorrowedBooks
                            string deleteBorrowedQuery = "DELETE FROM BorrowedBooks WHERE UserID = @UserID AND BookID = @BookID";

                            using (SqlCommand deleteBorrowedCommand = new SqlCommand(deleteBorrowedQuery, connection))
                            {
                                deleteBorrowedCommand.Parameters.AddWithValue("@UserID", userId);
                                deleteBorrowedCommand.Parameters.AddWithValue("@BookID", bookId);
                                deleteBorrowedCommand.ExecuteNonQuery();
                            }

                            // Update queue numbers only if more than one user is in the queue
                            if (numberInQueue > 1)
                            {
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

                            // Assign the book to the next user in line
                            BorrowBook(nextUserId, bookId);

                            message = $"Book returned and borrowed by the next user in line (UserID: {nextUserId}).";
                        }
                        else
                        {
                            // Close the reader before executing the next command
                            reader.Close();

                            // No one in the waiting list, increase available copies
                            string updateCopiesQuery = "UPDATE Books SET AvailableCopies = AvailableCopies + 1 WHERE Id = @BookID";

                            using (SqlCommand updateCopiesCommand = new SqlCommand(updateCopiesQuery, connection))
                            {
                                updateCopiesCommand.Parameters.AddWithValue("@BookID", bookId);
                                updateCopiesCommand.ExecuteNonQuery();
                            }

                            message = "Book returned successfully. Available copies increased.";
                        }
                    }
                    catch (Exception ex)
                    {
                        reader?.Close(); // Ensure the reader is closed in case of an exception
                        throw new Exception($"Error processing ReturnBook: {ex.Message}", ex);
                    }
                }
            }

            return message;
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
            string query = "SELECT age FROM Users WHERE id = @UserId";

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



    }


}
