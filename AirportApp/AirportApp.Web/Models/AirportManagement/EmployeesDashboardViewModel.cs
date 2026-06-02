using System.ComponentModel.DataAnnotations;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Web.Models.AirportManagement
{
    public class EmployeesDashboardViewModel
    {
        public List<Employee> Employees { get; set; } = new();
        public List<EmployeeRoleEnum> DisplayedRoles { get; set; } = new();
        public Employee? EditEmployee { get; set; }
    }

    public class EmployeeFormModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Salary must be a positive number.")]
        public int Salary { get; set; }

        [Required(ErrorMessage = "Birthday is required.")]
        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        [Required(ErrorMessage = "Hiring date is required.")]
        [DataType(DataType.Date)]
        public DateTime? HiringDate { get; set; }
    }
}

