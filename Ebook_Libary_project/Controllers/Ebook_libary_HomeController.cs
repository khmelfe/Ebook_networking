using EbookLibraryProject.Models;
using Ebook_Library_Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ebook_Libary_project.Controllers
{
    public class Ebook_libary_HomeController : Controller
    {
        //GET: Ebook_libary_Home

        static Ebook_libary_HomeController()
        {
            Usermodel.Initialize(1, "John Doe", "john.doe@mail.com", "securePass123", 25, false);
        }

        public ActionResult Ebook_home()
        {
            //Userdatabase.AddUser("Jane Doe", "jane.doe@mail.com", "securePass456", 30, false);

            // Step 2: Add book 6 to the books table
            Userdatabase.AddBook("Book 6", "Author 6", availableCopies: 3, buyPrice: 20.99m, borrowPrice: 4.99m);

            // Step 3: Add the new user to the waiting list for book 6
            Userdatabase.AddToWaitingList(userId: 2, bookId: 6);

            // Step 4: User 1 borrows book 6 with a return date of today
            Userdatabase.BorrowBook(userId: 1, bookId: 6);
           // Userdatabase.UpdateReturnDate(1, 6, 0);
            Console.WriteLine("Operations completed successfully.");
            return View("EbooK_home");
        }

       
    }

}
