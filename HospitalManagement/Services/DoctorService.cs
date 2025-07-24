using AutoMapper;
using HospitalManagement.Data;
using HospitalManagement.DTOs;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggingService _loggingService;
        private readonly IMapper _mapper;

        public DoctorService(
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
        public async Task<IEnumerable<DoctorDTO>> GetAllDoctorsAsync()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DoctorDTO>>(doctors);
        }
        public async Task<DoctorDTO> GetDoctorByIdAsync(int doctorId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            return doctor != null ? _mapper.Map<DoctorDTO>(doctor) : null;
        }
        public async Task<DoctorDTO> GetDoctorByUserIdAsync(string userId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            return doctor != null ? _mapper.Map<DoctorDTO>(doctor) : null;
        }
        public async Task<IEnumerable<DoctorDTO>> GetDoctorsByDepartmentAsync(int departmentId)
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .Where(d => d.DepartmentId == departmentId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<DoctorDTO>>(doctors);
        }
        public async Task<DoctorDTO> CreateDoctorAsync(DoctorDTO doctorDTO)
        {
            var user = await _userManager.FindByEmailAsync(doctorDTO.Email);
            if (user == null)
            {
                return null;
            }

            var doctor = new Doctor
            {
                UserId = user.Id,
                DepartmentId = doctorDTO.DepartmentId,
                Specialization = doctorDTO.Specialization,
                Qualifications = doctorDTO.Qualifications,
                LicenseNumber = doctorDTO.LicenseNumber,
                IsAvailable = doctorDTO.IsAvailable
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            await _loggingService.LogActivityAsync(
                user.Id,
                user.UserName,
                "Doctor",
                "Create",
                "Doctor",
                doctor.Id.ToString(),
                "Doctor profile created",
                "");

            return await GetDoctorByIdAsync(doctor.Id);
        }

        public async Task<DoctorDTO> UpdateDoctorAsync(int doctorId, DoctorDTO doctorDTO)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.Department)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null)
            {
                return null;
            }

            // Update doctor info
            doctor.DepartmentId = doctorDTO.DepartmentId;
            doctor.Specialization = doctorDTO.Specialization;
            doctor.Qualifications = doctorDTO.Qualifications;
            doctor.LicenseNumber = doctorDTO.LicenseNumber;
            doctor.IsAvailable = doctorDTO.IsAvailable;

            // Update ApplicationUser info
            doctor.User.FirstName = doctorDTO.FirstName;
            doctor.User.LastName = doctorDTO.LastName;
            doctor.User.PhoneNumber = doctorDTO.PhoneNumber;

            await _context.SaveChangesAsync();

            await _loggingService.LogActivityAsync(
                doctor.UserId,
                doctor.User.UserName,
                "Doctor",
                "Update",
                "Doctor",
                doctor.Id.ToString(),
                "Doctor profile updated",
                "");

            return _mapper.Map<DoctorDTO>(doctor);
        }

        public async Task<bool> DeleteDoctorAsync(int doctorId)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == doctorId);

            if (doctor == null)
            {
                return false;
            }

            string userId = doctor.UserId;
            string userName = doctor.User.UserName;

            // First delete doctor record
            _context.Doctors.Remove(doctor);

            // Then delete the user account
            var user = await _userManager.FindByIdAsync(doctor.UserId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            await _context.SaveChangesAsync();

            await _loggingService.LogActivityAsync(
                userId,
                userName,
                "Doctor",
                "Delete",
                "Doctor",
                doctorId.ToString(),
                "Doctor profile deleted",
                "");

            return true;
        }

      
    }
}
