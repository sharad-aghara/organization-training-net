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
        public int OrganizationId { get; set; }

        // Navigation property for Organization
        [ForeignKey("OrganizationId")]
        public Organization? Organization { get; set; }

        [Required]
        [StringLength(200)]
        public string PlaceOfTraining { get; set; }

        [Required]
        [StringLength(500)]
        public string PurposeOfTraining { get; set; }

        // Navigation property for related TrainingEmployee
        public ICollection<TrainingEmployee> TrainingEmployees { get; set; }

        // ctor
        public Training()
        {
            TrainingEmployees = new List<TrainingEmployee>();
        }
    }
}
