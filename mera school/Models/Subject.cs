using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mera_school.Models
{
    /// <summary>
    /// Represents a subject or course taught at the school.
    /// </summary>
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Subject Name is required.")]
        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject Code is required.")]
        [Display(Name = "Subject Code")]
        public string SubjectCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Credits must be provided.")]
        [Range(1, 10, ErrorMessage = "Credits must be a numeric value.")]
        public int Credits { get; set; }

        [Required(ErrorMessage = "Please assign a Teacher.")]
        [Display(Name = "Assigned Teacher")]
        public int TeacherId { get; set; }

        // Navigation property
        [ForeignKey("TeacherId")]
        public Teacher? Teacher { get; set; }
    }
}
