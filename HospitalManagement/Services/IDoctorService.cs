using HospitalManagement.DTOs;

namespace HospitalManagement.Services
{
    public interface IDoctorService
    {
        Task<IEnumerable<DoctorDTO>> GetAllDoctorsAsync();
        Task<DoctorDTO> GetDoctorByIdAsync(int doctorId);
        Task<DoctorDTO> GetDoctorByUserIdAsync(string userId);
        Task<IEnumerable<DoctorDTO>> GetDoctorsByDepartmentAsync(int departmentId);
        Task<DoctorDTO> CreateDoctorAsync(DoctorDTO doctorDTO);
        Task<DoctorDTO> UpdateDoctorAsync(int doctorId, DoctorDTO doctorDTO);
        Task<bool> DeleteDoctorAsync(int doctorId);

    }
}
