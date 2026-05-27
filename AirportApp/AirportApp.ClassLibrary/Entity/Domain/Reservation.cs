using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Reservations")]
    public class Reservation
    {
        [Key]
        [Column("Reservation_Id")]
        public int Id { get; set; }

        public Cart ReservationCart { get; set; }

        [Required]
        [Column("IsActive")]
        public bool Active { get; set; }

        [Required]
        [Column("Reservation_Date")]
        public DateTime ReservationDate { get; set; }

        public Reservation(int id, Cart reservationCart, bool active, DateTime reservationDate)
        {
            this.Id = id;
            this.ReservationCart = reservationCart;
            this.Active = active;
            this.ReservationDate = reservationDate;
        }

        public Reservation(Cart reservationCart, bool active, DateTime reservationDate)
        {
            this.ReservationCart = reservationCart;
            this.Active = active;
            this.ReservationDate = reservationDate;
        }

        internal Reservation()
        {
        }
    }
}
