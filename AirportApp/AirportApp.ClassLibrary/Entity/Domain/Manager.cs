using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace AirportApp.ClassLibrary.Entity.Domain
{
    [Table("Managers")]
    public class Manager
    {
        [Key]
        [Column("Manager_Id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        [Column("Email")]
        public string Email { get; set; }

        [MaxLength(20)]
        [Column("Phone")]
        public string Phone { get; set; }

        public Manager(int id, string name, string email, string phone)
        {
            this.Id = id;
            this.Name = name;
            this.Email = email;
            this.Phone = phone;
        }

        internal Manager()
        {
        }
    }
}
