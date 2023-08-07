namespace Mage.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        //Two parents (Cart and Game)
        public Cart? Cart { get; set; }
        public Game? Game  { get; set; }
    }
}