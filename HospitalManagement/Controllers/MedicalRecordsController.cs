using HospitalManagement.DTOs;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalRecordController : ControllerBase
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordController(IMedicalRecordService medicalRecordService)
        {
            _medicalRecordService = medicalRecordService;
        }

        // Get all medical records (only accessible by admin)
        [HttpGet]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllMedicalRecords()
        {
            var records = await _medicalRecordService.GetAllMedicalRecordsAsync();
            if (records == null || !records.Any())
            {
                return NotFound("No medical records found.");
            }

            return Ok(records);
        }

        // Get a medical record by its ID (accessible by doctor and admin)
        [HttpGet("{id}")]
       // [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetMedicalRecordById(int id)
        {
            var record = await _medicalRecordService.GetMedicalRecordByIdAsync(id);
            if (record == null)
            {
                return NotFound("Medical record not found.");
            }

            return Ok(record);
        }

        // Get medical records by patient ID (accessible by admin and the patient)
        [HttpGet("Patient/{patientId}")]
       // [Authorize(Roles = "Admin,Patient")]
        public async Task<IActionResult> GetMedicalRecordsByPatientId(int patientId)
        {
            var records = await _medicalRecordService.GetMedicalRecordsByPatientIdAsync(patientId);
            if (records == null || !records.Any())
            {
                return NotFound("No medical records found for this patient.");
            }

            return Ok(records);
        }

        // Get medical records by doctor ID (accessible by admin)
        [HttpGet("Doctor/{doctorId}")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMedicalRecordsByDoctorId(int doctorId)
        {
            var records = await _medicalRecordService.GetMedicalRecordsByDoctorIdAsync(doctorId);
            if (records == null || !records.Any())
            {
                return NotFound("No medical records found for this doctor.");
            }

            return Ok(records);
        }

        // Create a new medical record (accessible by doctor or admin)
        [HttpPost]
       // [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> CreateMedicalRecord([FromBody] MedicalRecordDTO medicalRecordDTO)
        {
            if (medicalRecordDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            var createdRecord = await _medicalRecordService.CreateMedicalRecordAsync(medicalRecordDTO);
            return CreatedAtAction(nameof(GetMedicalRecordById), new { id = createdRecord.Id }, createdRecord);
        }

        // Update an existing medical record (accessible by doctor or admin)
        [HttpPut("{id}")]
        //[Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> UpdateMedicalRecord(int id, [FromBody] MedicalRecordDTO medicalRecordDTO)
        {
            if (medicalRecordDTO == null)
            {
                return BadRequest("Invalid data.");
            }

            var updatedRecord = await _medicalRecordService.UpdateMedicalRecordAsync(id, medicalRecordDTO);
            if (updatedRecord == null)
            {
                return NotFound("Medical record not found.");
            }

            return Ok(updatedRecord);
        }

        // Delete a medical record (accessible by doctor or admin)
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> DeleteMedicalRecord(int id)
        {
            var isDeleted = await _medicalRecordService.DeleteMedicalRecordAsync(id);
            if (!isDeleted)
            {
                return NotFound("Medical record not found.");
            }

            return NoContent();
        }
        [HttpGet("MyRecords")]
       // [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetMyMedicalRecords()
        {
            var records = await _medicalRecordService.GetMyMedicalRecordsAsync();
            if (records == null || !records.Any())
            {
                return NotFound("No medical records found.");
            }

            return Ok(records);
        }


    }
}
