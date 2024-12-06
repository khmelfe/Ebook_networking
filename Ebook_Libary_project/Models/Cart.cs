using System;
using System.Collections.Generic;

namespace Ebook_Libary_project.Models
{
    public class Cart
    {
        // Key: BookId, Value: (Action, Price, Format)
        public Dictionary<int, (string Action, decimal Price, string Format)> Items { get; set; }

        // Constructor to initialize the cart
        public Cart()
        {
            Items = new Dictionary<int, (string Action, decimal Price, string Format)>();
        }

        private static Cart _cart = new Cart();

        // Provide access to the shared cart instance
        public static Cart GetCart()
        {
            return _cart;
        }

        // Method to add or update a book in the cart
        public void AddBookToCart(int bookId, string action, decimal price, string format)
        {
            if (Items.ContainsKey(bookId))
            {
                // Update the existing item in the cart
                Items[bookId] = (action, price, format);
            }
            else
            {
                // Add a new item to the cart
                Items.Add(bookId, (action, price, format));
            }
        }

        // Method to remove a book from the cart
        public void RemoveBookFromCart(int bookId)
        {
            if (Items.ContainsKey(bookId))
            {
                Items.Remove(bookId);
            }
        }

        // Method to calculate the total price of items in the cart
        public decimal CalculateTotalPrice()
        {
            decimal totalPrice = 0;
            foreach (var item in Items.Values)
            {
                totalPrice += item.Price;
            }
            return totalPrice;
        }
    }
}
