using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z][A-Za-z\s]*$", ErrorMessage = "Invalid Name")]
        public string Name { get; set; }

        // Foreign key to Organization
        //[ForeignKey("OrganizationId")]
        [Required(ErrorMessage = "Please select an organization.")]
        public int OrganizationId { get; set; }

        // Navigation property for Organization
        //[ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }

        // Navigation property for related TrainingEmployee
        public ICollection<TrainingEmployee> TrainingEmployees { get; set; }

        // ctor
        public Employee()
        {
            TrainingEmployees = new List<TrainingEmployee>();
        }
    }
}
