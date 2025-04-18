using AutoMapper;
using HospitalManagement.DTOs;
using HospitalManagement.Models;

namespace HospitalManagement.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {  // Patient mappings
            CreateMap<Patient, PatientDTO>();


            // Doctor mappings
            CreateMap<Doctor, DoctorDTO>();

            // Department mappings
            CreateMap<Department, DepartmentDTO>();

            // Appointment mappings
            CreateMap<Appointment, AppointmentDTO>();

            // MedicalRecord mappings
            CreateMap<MedicalRecord, MedicalRecordDTO>();

            // Billing mappings
            CreateMap<Billing, BillingDTO>();

            // User mappings
        }
    }
}
