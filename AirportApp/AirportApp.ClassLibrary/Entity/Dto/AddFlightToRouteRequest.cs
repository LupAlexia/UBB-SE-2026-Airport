namespace AirportApp.ClassLibrary.Entity.Dto
{
    public sealed class AddFlightToRouteRequest
    {
        public int CompanyId { get; set; }
        public int AirportId { get; set; }
        public string RouteType { get; set; } = string.Empty;
        public int RecurrenceInterval { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeOnly DepartureTime { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public int Capacity { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public int RunwayId { get; set; }
        public int GateId { get; set; }
    }
}