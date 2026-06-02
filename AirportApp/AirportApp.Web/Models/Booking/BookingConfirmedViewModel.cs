using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Models.Booking
{
    public class BookingConfirmedViewModel
    {
        public Flight Flight { get; set; } = null!;
        public int TicketCount { get; set; }
        public float TotalPaid { get; set; }
    }
}

