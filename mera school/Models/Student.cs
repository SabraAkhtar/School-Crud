using System.ComponentModel.DataAnnotations;

namespace mera_school.Models
{
    /// <summary>
    /// Represents a student enrolled in the school.
    /// </summary>
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name must contain only letters.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be at least 3 characters long and cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Age is required.")]
        [Range(5, 30, ErrorMessage = "Age must be between 5 and 30.")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Only numbers are allowed and it must be exactly 11 digits.")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Class name is required.")]
        [Display(Name = "Class")]
        public string ClassName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Admission date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Admission Date")]
        public DateTime AdmissionDate { get; set; } = DateTime.Today;

        [Display(Name = "Profile Image")]
        public string? StudentImage { get; set; }

        // Navigation property
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}
