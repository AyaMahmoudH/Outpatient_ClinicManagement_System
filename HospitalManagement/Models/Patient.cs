using XAct.Users;

namespace HospitalManagement.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string BloodType { get; set; }
        public string Allergies { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public string InsuranceInformation { get; set; }

        // Navigation properties
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Billing> Billings { get; set; }
        
        public DateTime RegistrationDate { get; internal set; }
    }
}
