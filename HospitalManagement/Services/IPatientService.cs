using AutoMapper;
using HospitalManagement.Data;
using HospitalManagement.DTOs;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using XAct.Messages;

public interface IPatientService
{
    Task<IEnumerable<PatientDTO>> GetAllPatientsAsync();
    Task<PatientDTO> GetPatientByIdAsync(int patientId);
    Task<PatientDTO> GetPatientByUserIdAsync(string userId);
    Task<PatientDTO> CreatePatientAsync(PatientDTO patientDTO);
    Task<PatientDTO> UpdatePatientAsync(int patientId, PatientDTO patientDTO);
    Task<bool> DeletePatientAsync(int patientId);

}
