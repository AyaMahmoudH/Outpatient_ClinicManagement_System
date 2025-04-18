using HospitalManagement.DTOs;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HospitalManagement.Data;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ApplicationDbContext _context;

        public AppointmentController(IAppointmentService appointmentService, ApplicationDbContext context)
        {
            _appointmentService = appointmentService;
            _context = context;
        }

        private string GetCurrentUserId()
        {
            return User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpGet]
       // [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return Ok(appointments);
        }

        [HttpGet("{id}")]
       // [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> GetById(int id)
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound();
            return Ok(appointment);
        }

        [HttpGet("patient/{patientId}")]
       // [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> GetByPatientId(int patientId)
        {
            var appointments = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId);
            return Ok(appointments);
        }

        [HttpGet("doctor/{doctorId}")]
        //[Authorize(Roles = "Admin,Doctor")]
        public async Task<IActionResult> GetByDoctorId(int doctorId)
        {
            var appointments = await _appointmentService.GetAppointmentsByDoctorIdAsync(doctorId);
            return Ok(appointments);
        }

        [HttpGet("date/{date}")]
       // [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> GetByDate(DateTime date)
        {
            var appointments = await _appointmentService.GetAppointmentsByDateAsync(date);
            return Ok(appointments);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> Create([FromBody] AppointmentDTO appointmentDTO)
        {
            var created = await _appointmentService.CreateAppointmentAsync(appointmentDTO);
            if (created == null)
                return BadRequest(new { message = "This time slot is already taken." });

            return Ok(created);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDTO appointmentDTO)
        {
            var currentUserId = GetCurrentUserId();

            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound(new { message = "Appointment not found." });

            if (User.IsInRole("Patient"))
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == appointment.PatientId);
                if (patient == null || patient.UserId != currentUserId)
                    return Forbid("You are not allowed to update this appointment.");
            }

            if (User.IsInRole("Doctor"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == appointment.DoctorId);
                if (doctor == null || doctor.UserId != currentUserId)
                    return Forbid("You are not allowed to update this appointment.");
            }

            var updated = await _appointmentService.UpdateAppointmentAsync(id, appointmentDTO);
            if (updated == null)
                return BadRequest(new { message = "This time slot is already taken or appointment not found." });

            return Ok(updated);
        }

        [HttpDelete("{id}")]
       // [Authorize(Roles = "Admin,Doctor,Patient")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var currentUserId = GetCurrentUserId();

            var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
            if (appointment == null)
                return NotFound(new { message = "Appointment not found." });

            if (User.IsInRole("Patient"))
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == appointment.PatientId);
                if (patient == null || patient.UserId != currentUserId)
                    return Forbid("You are not allowed to delete this appointment.");
            }

            if (User.IsInRole("Doctor"))
            {
                var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == appointment.DoctorId);
                if (doctor == null || doctor.UserId != currentUserId)
                    return Forbid("You are not allowed to delete this appointment.");
            }

            var result = await _appointmentService.DeleteAppointmentAsync(id);
            if (!result)
                return NotFound(new { message = "Appointment not found or already deleted." });

            return Ok(new { message = "Appointment deleted successfully." });
        }
    }
}
