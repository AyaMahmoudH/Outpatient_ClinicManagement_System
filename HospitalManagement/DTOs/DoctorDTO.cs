using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.DTOs
{
    public class DoctorDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Specialization { get; set; }
        public string Qualifications { get; set; }
        public string LicenseNumber { get; set; }
        public bool IsAvailable { get; set; }
    }
    public class DoctorRegistrationDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public string Specialization { get; set; }

        public string Qualifications { get; set; }

        [Required]
        public string LicenseNumber { get; set; }
    }
}
