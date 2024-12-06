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
            routes.MapRoute(
              name: "CartView",
              url: "User/Cart/{action}/{id}",
              defaults: new { controller = "Cart", action = "CartView", id = UrlParameter.Optional }
             );

            routes.MapRoute(
                name: "BookPage",
                url: "User/{action}/{id}",
                defaults: new { controller = "User", action = "bookPage", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Payment",
                url: "User/Payment/{action}/0",
                defaults: new { controller = "Payment", action = "Paymentvi", id = UrlParameter.Optional }
            );

            routes.MapRoute(
             name: "Default",
             url: "{controller}/{action}/{id}",
             defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

           


        }

    }
}
