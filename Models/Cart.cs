namespace MajorGamer.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public User? User { get; set; }
        //User is parent (can have multiple carts
        public List<CartItem>? CartItems { get; set; }
        public Order? Order { get; set; }
    }
}
