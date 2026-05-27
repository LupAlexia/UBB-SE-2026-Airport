using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("ShopItems")]
    public class ShopItem
    {
        public ShopItem(int id, int quantity, float price, Shop shop, string photo, string name, string description)
        {
            this.Id = id;
            this.Quantity = quantity;
            this.Price = price;
            this.Shop = shop;
            this.Photo = photo;
            this.Name = name;
            this.Description = description;
        }

        public ShopItem(int quantity, float price, Shop shop, string photo, string name, string description)
        {
            this.Id = 0;
            this.Quantity = quantity;
            this.Price = price;
            this.Shop = shop;
            this.Photo = photo;
            this.Name = name;
            this.Description = description;
        }

        public ShopItem()
        {
        }

        [Key]
        [Column("ShopItem_Id")]
        public int Id { get; set; }

        [Required]
        [Column("Quantity")]
        public int Quantity { get; set; }

        [Required]
        [Column("Price")]
        public float Price { get; set; }

        public Shop Shop { get; set; } = null!;

        public string Photo { get; set; } = string.Empty;

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Description")]
        public string Description { get; set; } = string.Empty;
    }
}
