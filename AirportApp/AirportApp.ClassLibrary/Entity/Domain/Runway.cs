using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Runways")]
    public class Runway
    {
        [Key]
        [Column("Runway_Id")]
        public int Id { get; set; }

        [Required]
        [Column("Name")]
        public string Name { get; set; }

        [Required]
        [Column("Handle_Time")]
        public int HandleTime { get; set; }

        public Runway()
        {
        }
    }
}