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

        public int Sale { get; set; } // Unique identifier
        public string ImagePath { get; set; } // Path to the book's image
        public string Name { get; set; } // Book title
        public string Author { get; set; } // Author name
        public decimal BuyingPrice { get; set; } // Price for buying
        public decimal BorrowPrice { get; set; } // Price for borrowing
        public int AvailableCopies { get; set; } // Number of books available for borrowing
      

        // Constructor
        public Book()
        {
            
            Sale = 0;
        }
        public string GetName()
        {
            return Name; // You can add extra logic here if needed
        }

     


      
    }
}
