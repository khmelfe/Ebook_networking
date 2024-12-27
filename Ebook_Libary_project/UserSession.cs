using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ebook_Libary_project
{
    public class UserSession
    {
        public static int GetCurrentUserId()
        {
            if (HttpContext.Current.Request.Cookies["userId"] != null)
            {
                int.TryParse(HttpContext.Current.Request.Cookies["userId"].Value, out int userId);
                return userId;
            }
            return 0; // Default if user is not logged in
        }

    }
}