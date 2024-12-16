using System;
using System.Collections.Generic;
using System.Linq;
using global::Ebook_Libary_project.Models;

namespace Ebook_Libary_project
{
    public static class BookDatabase
    {
        // Static list of books that will act like a database
        public static List<Book> Books { get; private set; }

        // Static constructor to initialize the list of books
        static BookDatabase()
        {
            // Initialize the Books list with some sample data
            Books = new List<Book>
            {
                new Book
                {
                    Id = 1,
                    Name = "Book 1",
                    Author = "Author 1",
                    BuyingPrice = 19.99m,
                    BorrowPrice = 4.99m,
                    AvailableCopies = 3,
                    ImagePath = "book1.jpg",
                    WaitingList = new Queue<string>()
                },
                new Book
                {
                    Id = 2,
                    Name = "Book 2",
                    Author = "Author 2",
                    BuyingPrice = 24.99m,
                    BorrowPrice = 5.99m,
                    AvailableCopies = 2,
                    ImagePath = "book2.jpg",
                    WaitingList = new Queue<string>()
                },
                new Book
                {
                    Id = 3,
                    Name = "Book 3",
                    Author = "Author 3",
                    BuyingPrice = 15.99m,
                    BorrowPrice = 3.99m,
                    AvailableCopies = 5,
                    ImagePath = "book3.jpg",
                    WaitingList = new Queue<string>()
                },
                new Book
                {
                    Id = 4,
                    Name = "Book 4",
                    Author = "Author 4",
                    BuyingPrice = 20.99m,
                    BorrowPrice = 4.49m,
                    AvailableCopies = 1,
                    ImagePath = "book4.jpg",
                    WaitingList = new Queue<string>()
                },
                new Book
                {
                    Id = 5,
                    Name = "Book 5",
                    Author = "Author 5",
                    BuyingPrice = 29.99m,
                    BorrowPrice = 6.99m,
                    AvailableCopies = 4,
                    ImagePath = "book5.jpg",
                    WaitingList = new Queue<string>()
                }
            };

        }

        // Improved method to get a book by ID using LINQ
        public static Book GetBookById(int id)
        {
            // Using LINQ to find the book by its ID
            return Books.FirstOrDefault(b => b.Id == id);
        }
        public static List<Book> getlist()
        {
            return Books;
        }

    }
}
