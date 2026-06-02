using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AirportApp.Web.Models.Booking
{
    public class BookingFormModel
    {
        [Required]
        public int FlightId { get; set; }

        public int RequestedPassengers { get; set; }

        public List<PassengerInputModel> Passengers { get; set; } = new List<PassengerInputModel>();
    }
}

