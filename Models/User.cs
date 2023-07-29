using Microsoft.AspNetCore.Identity;

namespace MajorGamer.Models
{
    //inherit from Identity User
    public class User : IdentityUser
    {
        public List<Cart>? Cart { get; set; }
        public List<Order>? Order { get; set; }
    }
}
