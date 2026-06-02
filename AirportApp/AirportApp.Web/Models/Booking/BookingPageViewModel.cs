using System.Collections.Generic;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Models.Booking
{
    public class BookingPageViewModel
    {
        public Flight Flight { get; set; } = null!;
        public float BasePrice { get; set; }
        public List<AddOn> AvailableAddOns { get; set; } = new List<AddOn>();
        public List<string> OccupiedSeats { get; set; } = new List<string>();
        public List<SeatDescriptor> SeatMapLayout { get; set; } = new List<SeatDescriptor>();
        public int SeatMapRowCount { get; set; }
        public int MaximumPassengers { get; set; }
        public BookingFormModel Form { get; set; } = new BookingFormModel();
        public string? ValidationMessage { get; set; }
        public float FlightDiscountPercentage { get; set; }
        public Dictionary<int, float> AddonDiscountPercentages { get; set; } = new Dictionary<int, float>();
    }
}

