using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Ebook_Library_Project
{
    public class Userdatabase
    {
        private static string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=User;Integrated Security=True";

        // Add a book to the bought list
        public void BuyBook(int bookId, int userId)
        {
            string query = "INSERT INTO BoughtBooks (UserID, BookID, PurchaseDate) VALUES (@UserID, @BookID, @PurchaseDate)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@UserID", userId);
                command.Parameters.AddWithValue("@BookID", bookId);
                command.Parameters.AddWithValue("@PurchaseDate", DateTime.Now);

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

        public string ReturnBook(int userId, int bookId)
        {
            string message = string.Empty;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if there is anyone in the waiting list for this book
                string waitingListQuery = "SELECT TOP 1 UserID FROM WaitingList WHERE BookID = @BookID ORDER BY NumberInQueue";

                using (SqlCommand waitingListCommand = new SqlCommand(waitingListQuery, connection))
                {
                    waitingListCommand.Parameters.AddWithValue("@BookID", bookId);

                    object nextUserIdObj = waitingListCommand.ExecuteScalar();

                    if (nextUserIdObj != null)
                    {
                        int nextUserId = Convert.ToInt32(nextUserIdObj);

                        // Update the queue numbers for all users in the waiting list for this book
                        string updateQueueQuery = "UPDATE WaitingList SET NumberInQueue = NumberInQueue - 1 WHERE BookID = @BookID";

                        using (SqlCommand updateQueueCommand = new SqlCommand(updateQueueQuery, connection))
                        {
                            updateQueueCommand.Parameters.AddWithValue("@BookID", bookId);
                            updateQueueCommand.ExecuteNonQuery();
                        }

                        // Remove the current entry from BorrowedBooks
                        string deleteBorrowedQuery = "DELETE FROM BorrowedBooks WHERE UserID = @UserID AND BookID = @BookID";

                        using (SqlCommand deleteBorrowedCommand = new SqlCommand(deleteBorrowedQuery, connection))
                        {
                            deleteBorrowedCommand.Parameters.AddWithValue("@UserID", userId);
                            deleteBorrowedCommand.Parameters.AddWithValue("@BookID", bookId);
                            deleteBorrowedCommand.ExecuteNonQuery();
                        }

                        // Call BorrowBook for the next user in line
                        BorrowBook(nextUserId, bookId);

                        message = $"Book returned and borrowed by the next user in line (UserID: {nextUserId}).";
                    }
                    else
                    {
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
            }

            return message;
        }

        // Method to borrow a book
        public void BorrowBook(int userId, int bookId)
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
        public void UpdateBookPrice(int bookId, decimal newPrice, string action)
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
        public void UpdateReturnDate(int userId, int bookId, int days)
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
        public void RemoveUser(int userId)
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
        public void AddBook(string title, string author, int availableCopies, decimal buyPrice, decimal borrowPrice)
        {
            string query = "INSERT INTO Books (Name, Author, AvailableCopies, BuyingPrice, BorrowPrice, Sale) VALUES (@Name, @Author, @AvailableCopies, @BuyingPrice, @BorrowPrice, 0)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", title);
                command.Parameters.AddWithValue("@Author", author);
                command.Parameters.AddWithValue("@AvailableCopies", availableCopies);
                command.Parameters.AddWithValue("@BuyingPrice", buyPrice);
                command.Parameters.AddWithValue("@BorrowPrice", borrowPrice);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }



        public void AddToWaitingList(int userId, int bookId)
        {
            string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=User;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if the user is already in the waiting list for the given book
                string checkQuery = "SELECT COUNT(*) FROM WaitingList WHERE UserID = @UserID AND BookID = @BookID";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@UserID", userId);
                    checkCommand.Parameters.AddWithValue("@BookID", bookId);
                    int count = (int)checkCommand.ExecuteScalar();

                    if (count > 0)
                    {
                        Debug.WriteLine("User is already in the waiting list for this book.");
                        return;
                    }
                }

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

        public void AddUser(string name,string mail,  string password, int age, Boolean admin)
        {

            string query = "INSERT INTO Users (name, mail, admin, age, password) VALUES (@Name, @Mail, @Admin, @Age, @Password)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Mail", mail);
                command.Parameters.AddWithValue("@Admin", admin);
                command.Parameters.AddWithValue("@Age", age);
                command.Parameters.AddWithValue("@Password", password);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }
}
