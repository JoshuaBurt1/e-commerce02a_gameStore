namespace Mage
{
    public class RouteConfig
    {
        public static void ConfigureRoutes(IEndpointRouteBuilder routes) 
        {

            routes.MapControllerRoute(
                name: "about",
                pattern: "about",
                defaults: new { controller = "Home", action = "About" } //route override
            ); //instantiating a new object

            routes.MapControllerRoute(
                name: "contact",
                pattern: "contact",
                defaults: new { controller = "Home", action = "Contact" });

            routes.MapControllerRoute( //WITHOUT THIS ROUTE: https://localhost:7150/Home/Privacy ;//WITH THIS ROUTE: https://localhost:7150/privacy
                name: "privacy",
                pattern: "privacy",
                defaults: new { controller = "Home", action = "Privacy" } //route override
            ); //instantiating a new object

            routes.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        }
    }
}
