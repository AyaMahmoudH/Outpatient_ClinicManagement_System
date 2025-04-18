using HospitalManagement.Data;
using HospitalManagement.DTOs;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggingService _loggingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public AppointmentService(
            ApplicationDbContext context,
            ILoggingService loggingService,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService)
        {
            _context = context;
            _loggingService = loggingService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        private async Task<ApplicationUser?> GetCurrentUser()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null || httpContext.User == null)
                return null;

            return await _userManager.GetUserAsync(httpContext.User);
        }

        private async Task<bool> IsDoctorAvailableAsync(int doctorId, object appointmentDateObj, object appointmentTimeObj)
        {
            var appointmentDate = Convert.ToDateTime(appointmentDateObj);
            var appointmentTime = (TimeSpan)appointmentTimeObj;

            return !await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == appointmentDate.Date &&
                a.AppointmentTime == appointmentTime);
        }

        private async Task<bool> IsDoctorAvailableAsync(int doctorId, object appointmentDateObj, object appointmentTimeObj, int excludeAppointmentId)
        {
            var appointmentDate = Convert.ToDateTime(appointmentDateObj);
            var appointmentTime = (TimeSpan)appointmentTimeObj;

            return !await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == appointmentDate.Date &&
                a.AppointmentTime == appointmentTime &&
                a.Id != excludeAppointmentId);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAllAppointmentsAsync()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .ToListAsync();

            return appointments.Select(MapToDTO);
        }

        public async Task<AppointmentDTO> GetAppointmentByIdAsync(int appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            return appointment != null ? MapToDTO(appointment) : null;
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Where(a => a.PatientId == patientId)
                .ToListAsync();

            return appointments.Select(MapToDTO);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Where(a => a.DoctorId == doctorId)
                .ToListAsync();

            return appointments.Select(MapToDTO);
        }

        public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByDateAsync(DateTime date)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Where(a => a.AppointmentDate.Date == date.Date)
                .ToListAsync();

            return appointments.Select(MapToDTO);
        }

        public async Task<AppointmentDTO> CreateAppointmentAsync(AppointmentDTO appointmentDTO)
        {
            bool isSlotAvailable = await IsDoctorAvailableAsync(
                appointmentDTO.DoctorId,
                appointmentDTO.AppointmentDate,
                appointmentDTO.AppointmentTime);

            if (!isSlotAvailable)
            {
                return null;
            }

            var appointment = new Appointment
            {
                PatientId = appointmentDTO.PatientId,

                DoctorId = appointmentDTO.DoctorId,
                AppointmentDate = (DateTime)appointmentDTO.AppointmentDate,
                AppointmentTime = (TimeSpan)appointmentDTO.AppointmentTime,
                Purpose = appointmentDTO.Purpose,
                Status = "Scheduled",
                Notes = appointmentDTO.Notes
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var user = await GetCurrentUser();

            await _loggingService.LogActivityAsync(
                user?.Id ?? "System",
                user?.UserName ?? "System",
                user != null ? (await _userManager.GetRolesAsync(user)).FirstOrDefault() : "System",
                "Create",
                "Appointment",
                appointment.Id.ToString(),
                $"Appointment scheduled for patient {appointmentDTO.PatientId} with doctor {appointmentDTO.DoctorId}",
                _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "");

            var patient = await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == appointmentDTO.PatientId);

            if (patient != null)
            {
                await _notificationService.CreateNotificationAsync(
                    patient.UserId,
              $"You have a new appointment scheduled for {((DateTime)appointmentDTO.AppointmentDate).ToShortDateString()} at {appointmentDTO.AppointmentTime}.",
                    "Appointment",
                    appointment.Id.ToString());

                await _notificationService.SendRealtimeNotificationAsync(
                    patient.UserId,
               $"You have a new appointment scheduled for {((DateTime)appointmentDTO.AppointmentDate).ToShortDateString()} at {appointmentDTO.AppointmentTime}.");
            }

            var doctor = await _context.Doctors.Include(d => d.User).FirstOrDefaultAsync(d => d.Id == appointmentDTO.DoctorId);

            if (doctor != null)
            {
                await _notificationService.CreateNotificationAsync(
                    doctor.UserId,
                    $"You have a new appointment scheduled for {((DateTime)appointmentDTO.AppointmentDate).ToShortDateString()} at {appointmentDTO.AppointmentTime}.",
                    "Appointment",
                    appointment.Id.ToString());

                await _notificationService.SendRealtimeNotificationAsync(
                    doctor.UserId,
                  $"You have a new appointment scheduled for {((DateTime)appointmentDTO.AppointmentDate).ToShortDateString()} at {appointmentDTO.AppointmentTime}.");
            }

            return await GetAppointmentByIdAsync(appointment.Id);
        }

        public async Task<AppointmentDTO> UpdateAppointmentAsync(int appointmentId, AppointmentDTO appointmentDTO)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);

            if (appointment == null)
                return null;

            bool isSlotAvailable = await IsDoctorAvailableAsync(
                appointmentDTO.DoctorId,
                appointmentDTO.AppointmentDate,
                appointmentDTO.AppointmentTime,
                appointmentId);

            if (!isSlotAvailable)
                return null;

            appointment.PatientId = appointmentDTO.PatientId;
            appointment.DoctorId = appointmentDTO.DoctorId;
            appointment.AppointmentDate = (DateTime)appointmentDTO.AppointmentDate;
            appointment.AppointmentTime = (TimeSpan)appointmentDTO.AppointmentTime;
            appointment.Purpose = appointmentDTO.Purpose;
            appointment.Notes = appointmentDTO.Notes;
            appointment.Status = appointmentDTO.Status;

            await _context.SaveChangesAsync();

            return await GetAppointmentByIdAsync(appointment.Id);
        }

        public async Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);

            if (appointment == null)
                return false;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return true;
        }

        private AppointmentDTO MapToDTO(Appointment appointment)
        {
            return new AppointmentDTO
            {
                Id = appointment.Id,
                PatientId = (int)(appointment?.PatientId),
                DoctorId = (int)appointment?.DoctorId,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Purpose = appointment.Purpose,
                Notes = appointment.Notes,
                Status = appointment.Status,
                PatientName = appointment.Patient?.User?.FullName,
                DoctorName = appointment.Doctor?.User?.FullName
            };
        }
    }
}
