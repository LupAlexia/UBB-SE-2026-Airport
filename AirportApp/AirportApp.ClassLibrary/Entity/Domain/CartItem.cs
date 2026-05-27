using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("CartItems")]
    public class CartItem
    {
        [Key]
        [Column("CartItem_Id")]
        public int Id { get; set; }

        public ShopItem ShopItem { get; set; } = null!;

        [Required]
        [Column("Quantity")]
        public int Quantity { get; set; }

        public CartItem(int id, ShopItem shopItem, int quantity)
        {
            this.Id = id;
            this.ShopItem = shopItem;
            this.Quantity = quantity;
        }

        internal CartItem()
        {
        }

        public float GetTotalPrice()
        {
            return ShopItem.Price * Quantity;
        }
    }
}
