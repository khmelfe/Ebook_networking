using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EbookLibraryProject.Models
{
    public class Usermodel
    {
        // Properties
        public int Id { get; set; }
        public bool admin { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
        public List<int> BoughtBookIds { get; set; } = new List<int>();
        public Dictionary<int, int> BorrowedBooks { get; set; } = new Dictionary<int, int>(); // Key: Book ID, Value: Remaining Time

        // Constructor
        public Usermodel(int id, string name, string password, int age)
        {
            Id = id;
            Name = name;
            Password = password;
            Age = age;
        }

        // Methods

        // Add a book to the bought list
        public void BuyBook(int bookId)
        {
            if (!BoughtBookIds.Contains(bookId))
            {
                BoughtBookIds.Add(bookId);
                Debug.WriteLine($"Book with ID {bookId} has been added to your bought list.");
            }
            else
            {
                Debug.WriteLine($"You already own the book with ID {bookId}.");
            }
        }

        // Borrow a book with a specific time
        public bool BorrowBook(int bookId, int time)
        {
            // If the book is not already borrowed, add it with the borrow time
            if (!BorrowedBooks.ContainsKey(bookId))
            {
                BorrowedBooks.Add(bookId, time);
                Debug.WriteLine($"Book with ID {bookId} borrowed for {time} days.");
                Debug.WriteLine(BorrowedBooks.Count);
                return true;  // Successfully borrowed a new book
            }
            else
            {
                // Do nothing if the book is already borrowed
                Debug.WriteLine($"You have already borrowed the book with ID {bookId}. You cannot borrow it again.");
                return false;  // Book already borrowed
            }
        }


        // Return a borrowed book
        public void ReturnBook(int bookId)
        {
            if (BorrowedBooks.ContainsKey(bookId))
            {
                BorrowedBooks.Remove(bookId);
                Debug.WriteLine($"Book with ID {bookId} has been returned.");
            }
            else
            {
                Debug.WriteLine($"You did not borrow the book with ID {bookId}.");
            }
        }

        // Reduce borrow time for all books (e.g., daily update)
        public void UpdateBorrowTime()
        {
            var booksToReturn = new List<int>();

            foreach (var book in BorrowedBooks)
            {
                BorrowedBooks[book.Key]--;
                if (BorrowedBooks[book.Key] <= 0)
                {
                    booksToReturn.Add(book.Key);
                }
            }

            foreach (var bookId in booksToReturn)
            {
                BorrowedBooks.Remove(bookId);
                Debug.WriteLine($"Book with ID {bookId} has reached its borrow limit and is automatically returned.");
            }
        }

        // Display user information
        public void DisplayUserInfo()
        {
            Debug.WriteLine($"ID: {Id}\nName: {Name}\nAge: {Age}\n");
            Debug.WriteLine("Bought Books:");
            foreach (var bookId in BoughtBookIds)
            {
                Debug.WriteLine($"- Book ID: {bookId}");
            }

            Debug.WriteLine("Borrowed Books:");
            foreach (var book in BorrowedBooks)
            {
                Debug.WriteLine($"- Book ID: {book.Key}, Remaining Time: {book.Value} days");
            }
        }

        // Show borrowed books
        public void ShowBorrowedBooks()
        {
            Debug.WriteLine("Borrowed Books:");
            if (BorrowedBooks.Count == 0)
            {
                Debug.WriteLine("No borrowed books.");
            }
            else
            {
                foreach (var book in BorrowedBooks)
                {
                    Debug.WriteLine($"BORROWED - Book ID: {book.Key}, Remaining Time: {book.Value} days");
                }
            }
        }

        // Show bought books
        public void ShowBoughtBooks()
        {
            Debug.WriteLine("Bought Books:");
            if (BoughtBookIds.Count == 0)
            {
                Debug.WriteLine("No bought books.");
            }
            else
            {
                foreach (var bookId in BoughtBookIds)
                {
                    Debug.WriteLine($"- Book ID: {bookId}");
                }
            }
        }
    }
}
