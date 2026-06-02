using System.ComponentModel.DataAnnotations;

namespace AirportApp.Web.Models.Auth;

public class EmployeeLoginViewModel
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Employee id must be a positive number.")]
    [Display(Name = "Employee ID")]
    public int EmployeeId { get; set; }

    [Required]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = string.Empty;
}
