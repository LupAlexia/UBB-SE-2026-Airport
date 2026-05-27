namespace AirportApp.ClassLibrary.Entity.Domain
{
    public sealed class EmployeeScheduleItem
    {
        public string Id { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string FlightType { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string GateName { get; set; } = string.Empty;
        public string RunwayName { get; set; } = string.Empty;
        public string FlightTime { get; set; } = string.Empty;
        public EmployeeScheduleItem()
        {
        }
    }
}