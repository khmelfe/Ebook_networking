using Ebook_Libary_project.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EbookLibraryProject.Models;

using System.Diagnostics;
using Microsoft.Ajax.Utilities;
using Ebook_Library_Project;
using System;


namespace Ebook_Libary_project.Controllers.user
{
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            List<int> userIds = Userdatabase.getallusersid();
            var model = new DashboardViewModel
            {
                UserCount = Userdatabase.Amount_of_users(),
                BookAmount = Userdatabase.Amount_of_books(),
                Userids = userIds,//all users ids.
            };



            return View("~/Views/User/Dashboard.cshtml", model);
        }
        public ActionResult GetUserDetails(int userId)
        {

            bool user = Userdatabase.userexistbyid(userId);

            // Check if user is found
            if (user)
            {
                // Add the user details to ViewBag or ViewModel
                ViewBag.Username = Userdatabase.GetUserNameById(userId);
                ViewBag.Email = Userdatabase.GetUserEmailById(userId);
                ViewBag.BorrowedBooks = Userdatabase.GetBorrowedBookIdsByUser(userId);
                ViewBag.PurchasedBooks = Userdatabase.GetBoughtBookIdsByUser(userId);
                ViewBag.IsAdmin = Userdatabase.IsUser_admin(userId);

                return PartialView("~/Views/User/_UserDetailsPartial.cshtml");
            }

            // Return a view with an error or empty data if user is not found
            return PartialView("~/Views/Shared/_ErrorPartial.cshtml");
        }

        public JsonResult SearchUsers(string searchTerm)
        {
            // Fetch users matching the search term from the database
            var users = Userdatabase.GetUsersByUsername(searchTerm);

            // Return the list of matching users in JSON format
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
                bool isGranted = Userdatabase.GrantAdminById(userId); // Replace with your actual logic

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
    }
}