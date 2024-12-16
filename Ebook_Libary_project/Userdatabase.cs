using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Ebook_Libary_project
{
    public class Userdatabase
    {



        string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=User;Integrated Security=True";

        // Add a book to the bought list
        public void BuyBook(int bookId,int userId)
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
            string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=User;Integrated Security=True";

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
            string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=User;Integrated Security=True";

            string query = "INSERT INTO BorrowedBooks (UserID, BookID ReturnDate) VALUES (@UserID, @BookID, @BorrowDate, @ReturnDate)";

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

    }
}