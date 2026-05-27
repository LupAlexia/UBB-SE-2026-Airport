namespace AirportApp.ClassLibrary.Entity.Domain
{
    public class FlightSummary
    {
        public int Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string DateText { get; set; } = string.Empty;
        public string DestinationText { get; set; } = string.Empty;
        public string RunwayText { get; set; } = string.Empty;
        public string GateText { get; set; } = string.Empty;
        public string CrewText { get; set; } = string.Empty;
    }
}
