using System.ComponentModel.DataAnnotations;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Models.Staff;

public class AddFlightFormModel
{
    [Required(ErrorMessage = "Please select a company.")]
    public int CompanyId { get; set; }

    [Required(ErrorMessage = "Please select a route type.")]
    public string RouteType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a destination airport.")]
    public int AirportId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1.")]
    public int Capacity { get; set; }

    [Required]
    [Range(0, 1439, ErrorMessage = "Departure time must be a valid minute offset.")]
    public int DepartureOffsetMinutes { get; set; }

    [Required]
    [Range(0, 1439, ErrorMessage = "Arrival time must be a valid minute offset.")]
    public int ArrivalOffsetMinutes { get; set; }

    public bool IsRecurrent { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? SingleDate { get; set; }

    public string RecurrenceType { get; set; } = "Daily";

    public string? CustomDaysText { get; set; }

    [Required(ErrorMessage = "Please select a runway.")]
    public int RunwayId { get; set; }

    [Required(ErrorMessage = "Please select a gate.")]
    public int GateId { get; set; }

    public string FlightNumberPrefix { get; set; } = string.Empty;

    public int DepartureHour { get; set; } = 12;
    public int DepartureMinute { get; set; } = 0;
    public string DepartureAmPm { get; set; } = "PM";

    public int ArrivalHour { get; set; } = 1;
    public int ArrivalMinute { get; set; } = 0;
    public string ArrivalAmPm { get; set; } = "PM";

    public List<Airport> Airports { get; set; } = new();
    public List<Runway> Runways { get; set; } = new();
    public List<Gate> Gates { get; set; } = new();
}
