using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Organization
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Navigation property for related Employees
        public ICollection<Employee> Employees { get; set; }

        public ICollection<Training> Training { get; set; }

        // Constructor
        public Organization()
        {
            Employees = new List<Employee>();
            Training = new List<Training>();
        }
    }

}
