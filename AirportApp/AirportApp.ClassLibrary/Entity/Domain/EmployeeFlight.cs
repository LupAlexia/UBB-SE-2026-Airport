using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("EmployeeFlights")]
    public class EmployeeFlight
    {
        public Employee Employee { get; set; }
        public Flight Flight { get; set; }

        public EmployeeFlight()
        {
        }
    }
}
