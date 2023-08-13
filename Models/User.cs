using Microsoft.AspNetCore.Identity;

namespace Mage.Models
{
    //inherit from Identity User
    public class User : IdentityUser
    {
        //child reference (set foreign keys)
        public List<Cart>? Cart { get; set; }
        public List<Order>? Order { get; set; }
    }
}
