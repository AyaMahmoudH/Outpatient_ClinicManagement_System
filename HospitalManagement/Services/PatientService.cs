using HospitalManagement.Data;
using AutoMapper;
using HospitalManagement.DTOs;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Models;

using XAct.Messages;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagement.Services
{
    public class PatientService : IPatientService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggingService _loggingService;
        private readonly IMapper _mapper;

        public PatientService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILoggingService loggingService,
             IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _loggingService = loggingService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PatientDTO>> GetAllPatientsAsync()
        {
            var patients = await _context.Patients
                .Include(p => p.User)
                .ToListAsync();

            return _mapper.Map<IEnumerable<PatientDTO>>(patients);

        }

        public async Task<PatientDTO> GetPatientByIdAsync(int patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            return patient != null ? _mapper.Map<PatientDTO>(patient) : null;
        }

        public async Task<PatientDTO> GetPatientByUserIdAsync(string userId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return patient != null ? _mapper.Map<PatientDTO>(patient) : null;
        }

        public async Task<PatientDTO> CreatePatientAsync(PatientDTO patientDTO)
        {
            var user = await _userManager.FindByEmailAsync(patientDTO.Email);
            if (user == null)
            {
                return null;
            }

            var patient = new Patient
            {
                UserId = user.Id,
                EmergencyContact = patientDTO.EmergencyContact,
                EmergencyContactPhone = patientDTO.EmergencyContactPhone,
                BloodType = patientDTO.BloodType,
                Allergies = patientDTO.Allergies,
                DateOfBirth = patientDTO.DateOfBirth,
                Address = patientDTO.Address,
                InsuranceInformation = patientDTO.InsuranceInformation
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            await _loggingService.LogActivityAsync(
                user.Id,
                user.UserName,
                "Patient",
                "Create",
                "Patient",
                patient.Id.ToString(),
                "Patient profile created",
                "");

            return await GetPatientByIdAsync(patient.Id);
        }
        public async Task<PatientDTO> UpdatePatientAsync(int patientId, PatientDTO patientDTO)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null)
            {
                return null;
            }

            // Update patient info
            patient.EmergencyContact = patientDTO.EmergencyContact;
            patient.EmergencyContactPhone = patientDTO.EmergencyContactPhone;
            patient.BloodType = patientDTO.BloodType;
            patient.Allergies = patientDTO.Allergies;
            patient.DateOfBirth = patientDTO.DateOfBirth;
            patient.Address = patientDTO.Address;
            patient.InsuranceInformation = patientDTO.InsuranceInformation;

            // Update ApplicationUser info
            patient.User.FirstName = patientDTO.FirstName;
            patient.User.LastName = patientDTO.LastName;
            patient.User.PhoneNumber = patientDTO.PhoneNumber;

            await _context.SaveChangesAsync();

            await _loggingService.LogActivityAsync(
                patient.UserId,
                patient.User.UserName,
                "Patient",
                "Update",
                "Patient",
                patient.Id.ToString(),
                "Patient profile updated",
                "");

            return _mapper.Map<PatientDTO>(patient);
        }

        public async Task<bool> DeletePatientAsync(int patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == patientId);

            if (patient == null)
            {
                return false;
            }

            string userId = patient.UserId;
            string userName = patient.User.UserName;

            // First delete patient record
            _context.Patients.Remove(patient);

            // Then delete the user account
            var user = await _userManager.FindByIdAsync(patient.UserId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            await _context.SaveChangesAsync();

            await _loggingService.LogActivityAsync(
                userId,
                userName,
                "Patient",
                "Delete",
                "Patient",
                patientId.ToString(),
                "Patient profile deleted",
                "");

            return true;
        }

      
    }
}
