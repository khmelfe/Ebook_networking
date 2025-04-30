using Ebook_Libary_project.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EbookLibraryProject.Models;

using System.Diagnostics;
using Microsoft.Ajax.Utilities;
using Ebook_Library_Project;
using System;
using System.Web;
using System.IO;


namespace Ebook_Libary_project.Controllers.user
{
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {

            return View("~/Views/User/Dashboard.cshtml");
        }
        public  JsonResult  amountof_users()
        {
            int amount;
            try
            {
                amount = Userdatabase.Amount_of_users();
                return Json(new { success = true, count = amount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex });
            }
        }
        public JsonResult amountof_books()
        {
            int amount;
            try
            {
                amount = Userdatabase.Amount_of_books();
                return Json(new { success = true, count = amount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex });
            }
        }
        public JsonResult Purchasedamount()
        {
            int amount;
            try
            {
                amount = Userdatabase.Amount_of_books_purchased();
                return Json(new { success = true, count = amount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex });
            }
        }
        public JsonResult borrowedamount()
        {
            int amount;
            try
            {
                amount = Userdatabase.Amount_of_books_inborrow_status();
                return Json(new { success = true, count = amount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex });
            }
        }
        public JsonResult waitingamount()
        {
            int amount;
            try
            {
                amount = Userdatabase.Amount_of_Waitinglists();
                return Json(new { success = true, count = amount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, ex });
            }
        }
        public ActionResult GetUserDetails(int userId)
        {

            bool user = Userdatabase.userexistbyid(userId);

            if (user)
            {
                ViewBag.Username = Userdatabase.GetUserNameById(userId);
                ViewBag.Email = Userdatabase.GetUserEmailById(userId);
                ViewBag.BorrowedBooks = Userdatabase.GetBorrowedBookIdsByUser(userId);
                ViewBag.PurchasedBooks = Userdatabase.GetBoughtBookIdsByUser(userId);
                ViewBag.IsAdmin = Userdatabase.IsUser_admin(userId);

                return PartialView("~/Views/User/_UserDetailsPartial.cshtml");
            }

            return PartialView("~/Views/Shared/_ErrorPartial.cshtml");
        }

        public JsonResult SearchUsers(string searchTerm)
        {

            var users = Userdatabase.GetUsersByUsername(searchTerm);

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            if (Userdatabase.userexistbyid(id))
            {
                Userdatabase.RemoveUser(id);
                return Json(new { success = true, message = "User deleted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Failed to delete user." });
            }
        }
        [HttpPost]
        public ActionResult GrantAdmin(int userId)
        {
            if (Userdatabase.userexistbyid(userId))
            {
                bool isGranted = Userdatabase.GrantAdminById(userId); 

                if (isGranted)
                {
                    return Json(new { success = true, message = "Admin privileges granted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to grant admin privileges." });
                }
            }
            return Json(new { success = false, message = "User not found." });
        }


        //Books functions

        [HttpPost]
        public JsonResult Addingbook(
       string title,
       string author,
       string category,
       int availableCopies,
       decimal buyPrice,
       decimal borrowPrice,
       int age = 0,
       HttpPostedFileBase imageFile = null,
       HttpPostedFileBase pdffile = null,
       HttpPostedFileBase mobifile = null,
       HttpPostedFileBase f2bfile = null,
       HttpPostedFileBase epubfile = null)
        {
            try
            {
                // Process the files and other parameters
                if (imageFile != null)
                {
                    //Image Book
                    string imagePath = Path.Combine(Server.MapPath("~/Content/Images"), Path.GetFileName(imageFile.FileName));
                    imageFile.SaveAs(imagePath);
                }
                //saving the books.
                if (pdffile != null)
                {
                    
                    string pdfPath = Path.Combine(Server.MapPath("~/Content/Books"), Path.GetFileName(pdffile.FileName));
                    pdffile.SaveAs(pdfPath);
                }

                if (mobifile != null)
                {
                    string mobiPath = Path.Combine(Server.MapPath("~/Content/Books"), Path.GetFileName(mobifile.FileName));
                    mobifile.SaveAs(mobiPath);
                }

                if (f2bfile != null)
                {
                    string f2bPath = Path.Combine(Server.MapPath("~/Content/Books"), Path.GetFileName(f2bfile.FileName));
                    f2bfile.SaveAs(f2bPath);
                }

                if (epubfile != null)
                {
                    string epubPath = Path.Combine(Server.MapPath("~/Content/Books"), Path.GetFileName(epubfile.FileName));
                    epubfile.SaveAs(epubPath);
                }

                Userdatabase.AddBook(title, author, category, availableCopies, buyPrice, borrowPrice, age, imageFile, pdffile, mobifile, f2bfile, epubfile);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GetAllBooks()
        {
            return Json(Userdatabase.GetAllBookIds()); 
        }
        public JsonResult SearchBooks(string searchTerm)
        {

            var users = Userdatabase.GetBooksBySearchTerm(searchTerm);

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult updateprice(int bookId, decimal newPrice, string action)
        {
            try
            {
                Userdatabase.UpdateBookPrice(bookId, newPrice, action);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    errorMessage = ex.Message,
                });
            }
        }
        public JsonResult saleprecent(int bookId, int sale)
        {
            try
            {
                Userdatabase.UpdateBookSale(bookId, sale);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    errorMessage = ex.Message,
                });
            }
        }
        public JsonResult Deletebook(int bookId)
        {
            try
            {
                Userdatabase.RemoveBook(bookId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    errorMessage = ex.Message,
                });
            }
        }
        //BorrowList!
        public JsonResult GetUsers_who_borrowed()
        {
            var users = Userdatabase.Users_with_borrowed_books();
            return Json(users, JsonRequestBehavior.AllowGet);
        }
        public JsonResult get_users_borrowed_books(int userid)
        {
            var books = Userdatabase.getusers_borrowed(userid);
            return Json(books, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateBorrowedTime(int userID, int bookId, int amount_of_days, string action)
        {
            try
            {
                Userdatabase.update_borrowed_time(userID, bookId, amount_of_days, action);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    errorMessage = ex.Message,
                });
            }
        }
        //WaitingList!
        public JsonResult GetbooksforWaiting()
        {
            var books = Userdatabase.GetBooksInWaitingList();
            return Json(books, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getusersofwaitinglist(int bookid)
        {
            var books = Userdatabase.GetUsersInWaitingList(bookid);
            return Json(books, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveFromWaitingListAndUpdateQueue(int bookId, int userId)
        {
            try
            {
                Userdatabase.RemoveFromWaitingListAndUpdateQueue(bookId, userId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    errorMessage = ex.Message,
                });
            }
        }

        public JsonResult GetAllCreditCards()
        {
            try
            {
                var cards = Userdatabase.GetAllCreditCards();
                return Json(new { success = true, cards = cards }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }

}


