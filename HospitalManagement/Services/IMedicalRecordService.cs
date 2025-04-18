using HospitalManagement.DTOs;

namespace HospitalManagement.Services
{
    public interface IMedicalRecordService
    {
        Task<IEnumerable<MedicalRecordDTO>> GetAllMedicalRecordsAsync();
        Task<MedicalRecordDTO> GetMedicalRecordByIdAsync(int recordId);
        Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByPatientIdAsync(int patientId);
        Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByDoctorIdAsync(int doctorId);
        Task<MedicalRecordDTO> CreateMedicalRecordAsync(MedicalRecordDTO recordDTO);
        Task<MedicalRecordDTO> UpdateMedicalRecordAsync(int recordId, MedicalRecordDTO recordDTO);
        Task<bool> DeleteMedicalRecordAsync(int recordId);
        Task<IEnumerable<MedicalRecordDTO>> GetMyMedicalRecordsAsync();

    }
}
