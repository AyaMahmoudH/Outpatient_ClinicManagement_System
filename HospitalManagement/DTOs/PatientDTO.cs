using System.ComponentModel.DataAnnotations;

namespace HospitalManagement.DTOs
{
    public class PatientDTO
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string BloodType { get; set; }
        public string Allergies { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string InsuranceInformation { get; set; }

    }
    public class PatientRegistrationDTO
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

        public string EmergencyContact { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string BloodType { get; set; }
        public string Allergies { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string InsuranceInformation { get; set; }
    }
}
