using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Models.Staff;

public class CrewManagementViewModel
{
    public int FlightId { get; set; }
    public FlightSummary? Flight { get; set; }
    public List<CrewMemberSelectionData> CrewCandidates { get; set; } = new();
    public List<int> SelectedEmployeeIds { get; set; } = new();
}
