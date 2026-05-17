using System.ComponentModel.DataAnnotations;

namespace mera_school.Models
{
    /// <summary>
    /// Represents a daily attendance record linking a student and a teacher.
    /// </summary>
    public class Attendance
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Student is required.")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Teacher is required.")]
        [Display(Name = "Teacher")]
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Attendance date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime AttendanceDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "Present";

        [StringLength(300)]
        public string? Remarks { get; set; }

        // Navigation properties
        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }
    }
}
