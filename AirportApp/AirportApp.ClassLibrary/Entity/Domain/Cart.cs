using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Carts")]
    public class Cart
    {
        [Key]
        [Column("Cart_Id")]
        public int Id { get; set; }

        public Client Client { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        internal Cart()
        {
        }

        public Cart(int id, Client client, ICollection<CartItem> cartItems)
        {
            this.Id = id;
            this.Client = client;
            this.CartItems = cartItems;
        }

        public float GetOverallPrice()
        {
            float overallPrice = 0;
            foreach (CartItem cartItem in CartItems)
            {
                overallPrice += cartItem.GetTotalPrice();
            }
            return overallPrice;
        }

        public void ClearCart()
        {
            CartItems.Clear();
        }

        public void UpdateQuantity(int cartItemId, int quantity)
        {
            var item = CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }

        public void AddCartItem(CartItem cartItem)
        {
            var existing = CartItems.FirstOrDefault(ci => ci.Id == cartItem.Id);
            if (existing != null)
            {
                existing.Quantity = cartItem.Quantity;
            }
            else
            {
                CartItems.Add(cartItem);
            }
        }

        public void RemoveCartItem(int cartItemId)
        {
            var item = CartItems.FirstOrDefault(ci => ci.Id == cartItemId);
            if (item != null)
            {
                CartItems.Remove(item);
            }
        }
    }
}