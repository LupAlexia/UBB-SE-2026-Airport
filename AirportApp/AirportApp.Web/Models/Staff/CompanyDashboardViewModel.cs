using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Models.Staff
{
    public class CompanyDashboardViewModel
    {
        public int SelectedCompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public List<Company> ManagerCompanies { get; set; } = new();
        public List<FlightSummary> Flights { get; set; } = new();
        public string SearchQuery { get; set; } = string.Empty;
        public AddFlightFormModel AddFlightForm { get; set; } = new();
        public bool ShowAddFlightForm { get; set; }
    }
}

