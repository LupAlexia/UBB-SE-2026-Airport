using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Models.AirportManagement
{
    public class StaffDashboardViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public List<EmployeeScheduleItem> Schedule { get; set; } = new();
    }
}

