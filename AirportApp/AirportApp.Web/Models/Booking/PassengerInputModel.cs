using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AirportApp.Web.Models.Booking
{
    public class PassengerInputModel
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        public string SelectedSeat { get; set; } = string.Empty;

        public List<int> SelectedAddOnIds { get; set; } = new List<int>();
    }
}

