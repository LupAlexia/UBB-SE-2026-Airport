 using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Routes")]
    public class Route
    {
        [Key]
        [Column("Route_Id")]
        public int Id { get; set; }
        public Company Company { get; set; } = null!;

        public Airport Airport { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        [Column("Route_Type")]
        public string RouteType { get; set; } = string.Empty;

        [Required]
        [Column("Departure_Date")]
        public DateOnly StartDate { get; set; }
        
        [Required]
        [Column("Arrival_Date")]
        public DateOnly EndDate { get; set; }

        [Required]
        [Column("Departure_Time")]
        public TimeOnly DepartureTime { get; set; }

        [Required]
        [Column("Arrival_Time")]
        public TimeOnly ArrivalTime { get; set; }

        [Column("Capacity")]
        public int Capacity { get; set; }

        [Column("Recurrence_Interval")]
        public int RecurrenceInterval { get; set; }

        public Route()
        {
        }

        // maybe we can eliminate this later, i don't think they're used anymore
        public Route(Company company, Airport airport, string routeType, DateOnly departureDate, DateOnly arrivalDate, TimeOnly departureTime, TimeOnly arrivalTime, int capacity, int flightRecurrenceInterval)
        {
            Company = company;
            Airport = airport;
            RouteType = routeType;
            StartDate = departureDate;
            EndDate = arrivalDate;
            Capacity = capacity;
            RecurrenceInterval = flightRecurrenceInterval;
        }

        public Route(int routeId, Company company, Airport airport, string routeType, DateOnly departureDate, DateOnly arrivalDate, TimeOnly departureTime, TimeOnly arrivalTime, int capacity, int flightRecurrenceInterval)
        {
            Id = routeId;
            Company = company;
            Airport = airport;
            RouteType = routeType;
            StartDate = departureDate;
            EndDate = arrivalDate;
            Capacity = capacity;
            RecurrenceInterval = flightRecurrenceInterval;
        }
    }
}
