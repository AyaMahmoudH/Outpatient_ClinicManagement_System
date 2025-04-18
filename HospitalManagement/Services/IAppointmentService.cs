using HospitalManagement.DTOs;

namespace HospitalManagement.Services
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDTO>> GetAllAppointmentsAsync();
        Task<AppointmentDTO> GetAppointmentByIdAsync(int appointmentId);
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByPatientIdAsync(int patientId);
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDoctorIdAsync(int doctorId);
        Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDateAsync(DateTime date);
        Task<AppointmentDTO> CreateAppointmentAsync(AppointmentDTO appointmentDTO);
        Task<AppointmentDTO> UpdateAppointmentAsync(int appointmentId, AppointmentDTO appointmentDTO);
        Task<bool> DeleteAppointmentAsync(int appointmentId);
    }
}
