namespace HospitalManagement.DTOs
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime RecordDate { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Prescription { get; set; }
        public string Notes { get; set; }
        public string FollowUpInstructions { get; set; }
        public DateTime AppointmentDate { get; internal set; }
        public TimeSpan AppointmentTime { get; internal set; }
        public string Purpose { get; internal set; }
        public string Status { get; internal set; }
    }
   
}
