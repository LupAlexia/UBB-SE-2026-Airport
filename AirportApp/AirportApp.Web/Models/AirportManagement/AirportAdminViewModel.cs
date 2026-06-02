using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Models.AirportManagement
{
    public enum AirportAdministrationSection
    {
        Flights,
        Employees,
        Configuration
    }

    public class AirportAdminViewModel
    {
        public AirportAdministrationSection ActiveSection { get; set; } = AirportAdministrationSection.Flights;

        public FlightsDashboardViewModel FlightsDashboard { get; set; } = new();

        public EmployeesDashboardViewModel EmployeesDashboard { get; set; } = new();

        public List<Runway> RunwaysList { get; set; } = new();

        public List<Gate> GatesList { get; set; } = new();

        public List<Airport> AirportsList { get; set; } = new();
    }
}
