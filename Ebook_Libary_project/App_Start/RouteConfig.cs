using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ebook_Libary_project
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Home_page",
                url: "Ebook_home",
                defaults: new {controller = "Ebook_libary_HomeController", action = "Ebook_home",id = UrlParameter.Optional }

                );

            routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new { Controller = "LoginController", action = "Login", id = UrlParameter.Optional }
                );
        }
    }
}
