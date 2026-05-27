using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        [Column("Customer_Id")]
        public int Id { get; set; }

        [Required]
        [Column("Role")]
        public EmployeeRoleEnum Role { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Name")]
        public string Name { get; set; }

        [Required]
        [Column("Date_of_Birth")]
        public DateOnly Birthday { get; set; }

        [Required]
        [Column("Hire_Date")]
        public DateOnly HiringDate { get; set; }

        [Required]
        [Column("Salary")]
        public int Salary { get; set; }
        public Employee()
        {
        }

        public Employee(string name, EmployeeRoleEnum role)
        {
            this.Name = name;
            this.Role = role;
            this.Birthday = DateOnly.FromDateTime(DateTime.Now.AddYears(-18));
            this.HiringDate = DateOnly.FromDateTime(DateTime.Now);
            this.Salary = 0;
        }
    }
}
