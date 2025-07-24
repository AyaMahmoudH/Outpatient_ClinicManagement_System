using AutoMapper;
using HospitalManagement.Data;
using HospitalManagement.DTOs;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Services
{
    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggingService _loggingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;

        public MedicalRecordService(
            ApplicationDbContext context,
            ILoggingService loggingService,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService,
            IMapper mapper)
        {
            _context = context;
            _loggingService = loggingService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MedicalRecordDTO>> GetAllMedicalRecordsAsync()
        {
            var records = await _context.MedicalRecords
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MedicalRecordDTO>>(records);
        }

        public async Task<MedicalRecordDTO> GetMedicalRecordByIdAsync(int recordId)
        {
            var record = await _context.MedicalRecords
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(r => r.Id == recordId);

            return record != null ? _mapper.Map<MedicalRecordDTO>(record) : null;
        }

        public async Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByPatientIdAsync(int patientId)
        {
            var records = await _context.MedicalRecords
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .Where(r => r.PatientId == patientId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MedicalRecordDTO>>(records);
        }

        public async Task<IEnumerable<MedicalRecordDTO>> GetMedicalRecordsByDoctorIdAsync(int doctorId)
        {
            var records = await _context.MedicalRecords
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .Where(r => r.DoctorId == doctorId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MedicalRecordDTO>>(records);
        }

        public async Task<MedicalRecordDTO> CreateMedicalRecordAsync(MedicalRecordDTO recordDTO)
        {
            var record = new MedicalRecord
            {
                PatientId = recordDTO.PatientId,
                DoctorId = recordDTO.DoctorId,
                RecordDate = recordDTO.RecordDate,
                Diagnosis = recordDTO.Diagnosis,
                Treatment = recordDTO.Treatment,
                Prescription = recordDTO.Prescription,
                Notes = recordDTO.Notes,
                FollowUpInstructions = recordDTO.FollowUpInstructions
            };

            _context.MedicalRecords.Add(record);
            await _context.SaveChangesAsync();

            // Get current user
            var user = await GetCurrentUser();

            // Log activity
            await _loggingService.LogActivityAsync(
                user?.Id ?? "System",
                user?.UserName ?? "System",
                "Doctor",
                "Create",
                "MedicalRecord",
                record.Id.ToString(),
                $"Medical record created for patient {recordDTO.PatientId}",
                _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "");

            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == recordDTO.PatientId);

            if (patient != null)
            {
                await _notificationService.CreateNotificationAsync(
                    patient.UserId,
                    "A new medical record has been added to your profile.",
                    "MedicalRecord",
                    record.Id.ToString());

                await _notificationService.SendRealtimeNotificationAsync(
                    patient.UserId,
                    "A new medical record has been added to your profile.");
            }

            return await GetMedicalRecordByIdAsync(record.Id);
        }

        public async Task<MedicalRecordDTO> UpdateMedicalRecordAsync(int recordId, MedicalRecordDTO recordDTO)
        {
            var record = await _context.MedicalRecords
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.Id == recordId);

            if (record == null)
            {
                return null;
            }

            record.DoctorId = recordDTO.DoctorId;
            record.RecordDate = recordDTO.RecordDate;
            record.Diagnosis = recordDTO.Diagnosis;
            record.Treatment = recordDTO.Treatment;
            record.Prescription = recordDTO.Prescription;
            record.Notes = recordDTO.Notes;
            record.FollowUpInstructions = recordDTO.FollowUpInstructions;

            await _context.SaveChangesAsync();

            // Get current user
            var user = await GetCurrentUser();

            // Log activity
            await _loggingService.LogActivityAsync(
                user?.Id ?? "System",
                user?.UserName ?? "System",
                "Doctor",
                "Update",
                "MedicalRecord",
                record.Id.ToString(),
                $"Medical record updated for patient {recordDTO.PatientId}",
                _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "");

            // Send notification to patient about updated medical record
            if (record.Patient?.UserId != null)
            {
                await _notificationService.CreateNotificationAsync(
                    record.Patient.UserId,
                    "Your medical record has been updated.",
                    "MedicalRecord",
                    record.Id.ToString());

                await _notificationService.SendRealtimeNotificationAsync(
                    record.Patient.UserId,
                    "Your medical record has been updated.");
            }

            return await GetMedicalRecordByIdAsync(record.Id);
        }

        public async Task<bool> DeleteMedicalRecordAsync(int recordId)
        {
            var record = await _context.MedicalRecords.FindAsync(recordId);
            if (record == null)
            {
                return false;
            }

            _context.MedicalRecords.Remove(record);
            await _context.SaveChangesAsync();

            // Get current user
            var user = await GetCurrentUser();

            // Log activity
            await _loggingService.LogActivityAsync(
                user?.Id ?? "System",
                user?.UserName ?? "System",
                "Doctor",
                "Delete",
                "MedicalRecord",
                recordId.ToString(),
                $"Medical record deleted for patient {record.PatientId}",
                _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "");

            return true;
        }

       
        public async Task<IEnumerable<MedicalRecordDTO>> GetMyMedicalRecordsAsync()
        {
            var currentUser = await GetCurrentUser();

            if (currentUser == null)
            {
                return null;
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.UserId == currentUser.Id);

            if (patient == null)
            {
                return null;
            }

            var records = await _context.MedicalRecords
                .Where(r => r.PatientId == patient.Id)
                .Include(r => r.Patient)
                    .ThenInclude(p => p.User)
                .Include(r => r.Doctor)
                    .ThenInclude(d => d.User)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MedicalRecordDTO>>(records);
        }
        private async Task<ApplicationUser> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return user;
        }
    

    }
}
