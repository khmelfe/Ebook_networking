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
       
            Userdatabase.AddBook("To Kill a Mockingbird", "Harper Lee", 10, 15.99m, 3.99m);
            Userdatabase.AddBook("1984", "George Orwell", 12, 12.99m, 2.99m);
            Userdatabase.AddBook("Pride and Prejudice", "Jane Austen", 8, 10.49m, 2.49m);
            Userdatabase.AddBook("The Great Gatsby", "F. Scott Fitzgerald", 5, 14.99m, 3.49m);
            Userdatabase.AddBook("Moby-Dick", "Herman Melville", 7, 11.99m, 2.99m);
        
            return View("EbooK_home");
        }

    }

}
