using System.Web.Mvc;
using System.Web.Routing;

public class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.MapRoute(
            name: "Paymentname",
            url: "Payment",
            defaults: new { controller = "Payment", action = "Paymentvi", id = UrlParameter.Optional }
        );

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
            name: "Cart",
            url: "User/Cart/{action}/{id}",
            defaults: new { controller = "Cart", action = "ViewCart", id = UrlParameter.Optional }
        );

        routes.MapRoute(
            name: "Books",
            url: "User/{action}/{id}",
            defaults: new { controller = "Books", action = "Details", id = UrlParameter.Optional }
        );

        // Main_Page
        routes.MapRoute(
            name: "Default",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Ebook_libary_Home", action = "Ebook_home", id = UrlParameter.Optional }
        );

        routes.MapRoute(
            name: "Login",
            url: "Login",
            defaults: new { controller = "Login", action = "Login", id = UrlParameter.Optional }
        );

        routes.MapRoute(
            name: "Dashboard",
            url: "Dashboard",
            defaults: new { controller = "Dashboard", action = "index", id = UrlParameter.Optional }
        );
    }
}
