using HospitalManagement.DTOs;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "Admin,Doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // GET: api/Doctor
        [HttpGet]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }

        // GET: api/Doctor/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        // GET: api/Doctor/User/{userId}
        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetDoctorByUserId(string userId)
        {
            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
            if (doctor == null)
                return NotFound();

            return Ok(doctor);
        }

        // GET: api/Doctor/Department/3
        [HttpGet("Department/{departmentId}")]
        public async Task<IActionResult> GetDoctorsByDepartment(int departmentId)
        {
            var doctors = await _doctorService.GetDoctorsByDepartmentAsync(departmentId);
            return Ok(doctors);
        }

        // POST: api/Doctor
        ////[HttpPost]
        ////[Authorize(Roles = "Admin")]
        ////public async Task<IActionResult> CreateDoctor([FromBody] DoctorDTO doctorDTO)
        ////{
        ////    var doctor = await _doctorService.CreateDoctorAsync(doctorDTO);
        ////    if (doctor == null)
        ////        return BadRequest("User not found or invalid data.");

        ////    return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, doctor);
        ////}

        // PUT: api/Doctor/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDoctor(int id, [FromBody] DoctorDTO doctorDTO)
        {
            var updatedDoctor = await _doctorService.UpdateDoctorAsync(id, doctorDTO);
            if (updatedDoctor == null)
                return NotFound();

            return Ok(updatedDoctor);
        }

        // DELETE: api/Doctor/5
        [HttpDelete("{id}")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var result = await _doctorService.DeleteDoctorAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        // GET: api/Doctor/MyProfile
        [HttpGet("MyProfile")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var doctor = await _doctorService.GetDoctorByUserIdAsync(userId);
            if (doctor == null)
                return NotFound("Doctor profile not found.");

            return Ok(doctor);
        }

    }
}
