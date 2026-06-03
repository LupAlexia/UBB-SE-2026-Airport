using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Models.AirportManagement
{
    public class EmployeeDashboardViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public IEnumerable<EmployeeScheduleItem> Schedule { get; set; } = new List<EmployeeScheduleItem>();
    }
}

