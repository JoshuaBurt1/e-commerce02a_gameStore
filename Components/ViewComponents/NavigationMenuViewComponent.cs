using Mage.Models;
using Microsoft.AspNetCore.Mvc; //needed for ViewComponent to be accessible
using System.Net.NetworkInformation;

namespace Mage.Components.ViewComponents
{
    public class NavigationMenuViewComponent : ViewComponent //inherit from above folder ViewComponents
    {
        //to render method
        public IViewComponentResult Invoke()
        {
            //Authorized = true only shows Menu items if authorized
            var menuItems = new List<MenuItem>
            {
                new MenuItem { Controller = "Home", Action = "Index", Label = "Home" },
                new MenuItem { Controller = "Shop", Action = "Index", Label = "Shop"},
                new MenuItem { Controller = "Categories", Action = "Index", Label = "Categories", DropdownItems = new
                List<MenuItem> { //creates drop down items in navigation bar
                    new MenuItem{Controller = "Categories", Action = "Index", Label = "List"} ,
                    new MenuItem{Controller = "Categories", Action = "Create", Label = "Create"} ,
                    }, Authorized = true, AllowedRoles = new List<string> { "Administrator"} },
                new MenuItem { Controller = "Games", Action = "Index", Label = "Games", DropdownItems = new
                List<MenuItem> { //creates drop down items in navigation bar
                    new MenuItem{Controller = "Games", Action = "Index", Label = "List"} ,
                    new MenuItem{Controller = "Games", Action = "Create", Label = "Create"} ,
                    }, Authorized = true, AllowedRoles = new List<string> { "Administrator"} }, 
                new MenuItem { Controller = "Home", Action = "About", Label = "About" },
                new MenuItem { Controller = "Home", Action = "Contact", Label = "Contact" },
                new MenuItem { Controller = "Home", Action = "Privacy", Label = "Privacy" },
            };
            return View(menuItems); //Becomes "Model" in the View
        }
    }
}
