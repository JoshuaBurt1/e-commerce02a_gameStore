using Microsoft.AspNetCore.Mvc;
using Mage.Models;

namespace Mage.Components.ViewComponents
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var menuItems = new List<MenuItem>
            {
                new MenuItem {Controller = "Home", Action = "Index", Label = "Home" },
                new MenuItem {Controller = "Shop", Action = "Index", Label = "Shop" },
                new MenuItem {Controller = "Categories", Action = "Index", Label = "Category", DropdownItems = new List<MenuItem>  {
                    new MenuItem {Controller = "Categories", Action = "Index", Label = "List"},
                    new MenuItem {Controller = "Categories", Action = "Create", Label = "Create"},
                }, Authorized = true, AllowedRoles = new List<string>{"Administrator"} },
                new MenuItem {Controller = "Games", Action = "Index", Label = "Game", DropdownItems = new List<MenuItem>  {
                    new MenuItem {Controller = "Games", Action = "Index", Label = "List"},
                    new MenuItem {Controller = "Games", Action = "Create", Label = "Create"},
                }, Authorized = true, AllowedRoles = new List<string>{"Administrator"} },
                new MenuItem {Controller = "Home", Action = "About", Label = "About"},
                new MenuItem {Controller = "Home", Action = "Contact", Label = "Contact"},
                new MenuItem {Controller = "Home", Action = "Privacy", Label = "Privacy" },
            };
            return View(menuItems);
        }
    }
}
