namespace HospitalManagement.Models
{
    public class Doctor
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public string Specialization { get; set; }
        public string Qualifications { get; set; }

        public string LicenseNumber { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<MedicalRecord> MedicalRecords { get; set; }
        public bool IsAvailable { get; internal set; }
    }
}
