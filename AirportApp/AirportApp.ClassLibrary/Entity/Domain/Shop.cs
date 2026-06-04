using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Shops")]
    public class Shop
    {
        [Key]
        [Column("Shop_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Name")]
        public string Name { get; set; }

        [Required]
        [Column("Type")]
        public string Type { get; set; }

        public Manager Manager { get; set; }

        public Shop()
        {
        }

        public Shop(int id, string name, string type, Manager manager)
        {
            this.Id = id;
            this.Name = name;
            this.Type = type;
            this.Manager = manager;
        }

        public Shop(string name, string type, Manager manager)
        {
            this.Name = name;
            this.Type = type;
            this.Manager = manager;
        }
    }
}
