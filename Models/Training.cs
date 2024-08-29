using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Training
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateOnly TrainingDate { get; set; }

        // Foreign key to Organization
        [Required(ErrorMessage = "Please select an organization.")]
        public int OrganizationId { get; set; }

        // Navigation property for Organization
        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }

        [Required]
        [StringLength(20)]
        public string PlaceOfTraining { get; set; }

        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Za-z0-9][A-Za-z0-9\s-]*$", ErrorMessage = "Invalid Pattern")]
        public string PurposeOfTraining { get; set; }

        // Navigation property for related TrainingEmployee
        [Required(ErrorMessage = "Please select an Employee.")]
        public ICollection<TrainingEmployee> TrainingEmployees { get; set; }

        // ctor
        public Training()
        {
            TrainingEmployees = new List<TrainingEmployee>();
        }
    }
}
