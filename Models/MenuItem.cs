namespace Mage.Models
{
    public struct MenuItem
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Label { get; set; }
        public List<MenuItem> DropdownItems { get; set; }

        //Is menu item authorized via login?
        public bool? Authorized { get; set; }
        public List<string?> AllowedRoles { get; set; }
    }
}
