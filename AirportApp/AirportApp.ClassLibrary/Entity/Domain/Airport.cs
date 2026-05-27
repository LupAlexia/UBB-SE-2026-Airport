using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Airports")]
    public class Airport
    {
        [Key]
        [Column("Airport_Id")]
        public int Id { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [Column("Airport_Code")]
        public string AirportCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("City")]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("Name")]
        public string Name { get; set; }

        public ICollection<Gate> Gates { get; set; } = new List<Gate>();
        public Airport()
        {
        }

        public Airport(string airportCode, string city, string name)
        {
            this.AirportCode = airportCode;
            this.City = city;
            this.Name= name;
        }

        public Airport(int airportId, string airportCode, string city, string name)
        {
            this.Id = airportId;
            this.AirportCode = airportCode;
            this.City = city;
            this.Name= name;
        }
    }
}
