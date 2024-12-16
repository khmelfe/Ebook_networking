using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ebook_Libary_project.Models

{
    public class Book
    {
        // Book properties
        public int Id { get; set; } // Unique identifier
        public string ImagePath { get; set; } // Path to the book's image
        public string Name { get; set; } // Book title
        public string Author { get; set; } // Author name
        public decimal BuyingPrice { get; set; } // Price for buying
        public decimal BorrowPrice { get; set; } // Price for borrowing
        public int AvailableCopies { get; set; } // Number of books available for borrowing
        public Queue<string> WaitingList { get; set; } // Queue for waiting list (customer IDs or names)

        // Constructor
        public Book()
        {
            WaitingList = new Queue<string>();
        }

        // Method to borrow a book
        public decimal BorrowBook(string customerId)
        {
            if (AvailableCopies > 0)
            {
                AvailableCopies--;
                
            }
            else
            {
                WaitingList.Enqueue(customerId);
               
            }
            return BorrowPrice;
        }

        public decimal BuyBook(string customerId)
        {
           
            return BuyingPrice;
        }
        // Method to return a book


      
    }
}
