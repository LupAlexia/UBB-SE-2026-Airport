using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Clients")]
    public class Client
    {
        [Key]
        [Column("Client_Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("Name")]
        public string Name { get; set; } = null!;

        public Client(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        internal Client()
        {
        }
    }
}
