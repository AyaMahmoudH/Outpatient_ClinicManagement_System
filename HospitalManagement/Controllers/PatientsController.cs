using HospitalManagement.DTOs;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,Doctor")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // GET: api/Patient
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }

        // GET: api/Patient/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        // GET: api/Patient/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var patient = await _patientService.GetPatientByUserIdAsync(userId);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

        // POST: api/Patient
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Create([FromBody] PatientDTO patientDto)
        //{
        //    var created = await _patientService.CreatePatientAsync(patientDto);
        //    if (created == null)
        //        return BadRequest("User with provided email not found.");

        //    return CreatedAtAction(nameof(GetById), new { id = created.PatientId }, created);
        //}

        // PUT: api/Patient/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Doctor,Nurse")]
        public async Task<IActionResult> Update(int id, [FromBody] PatientDTO patientDto)
        {
            var updated = await _patientService.UpdatePatientAsync(id, patientDto);
            if (updated == null)
                return NotFound();

            return Ok(updated);
        }

        // DELETE: api/Patient/{id}
        [HttpDelete("{id}")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _patientService.DeletePatientAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }
        [HttpGet("me")]
       // [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var patient = await _patientService.GetPatientByUserIdAsync(userId);
            if (patient == null)
                return NotFound();

            return Ok(patient);
        }

    }
}
