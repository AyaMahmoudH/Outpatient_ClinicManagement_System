using AutoMapper;
using HospitalManagement.DTOs;
using HospitalManagement.Models;

namespace HospitalManagement.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {  // Patient mappings
            CreateMap<Patient, PatientDTO>()
                       .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.Id))
                       .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                       .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                       .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                       .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));

            // Doctor mappings
            CreateMap<Doctor, DoctorDTO>()
    .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
    .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
    .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name));


            // Department mappings
            CreateMap<Department, DepartmentDTO>().ReverseMap();

            // Appointment mappings
            CreateMap<Appointment, AppointmentDTO>();

            // MedicalRecord mappings
            CreateMap<MedicalRecord, MedicalRecordDTO>()
                  .ForMember(dest => dest.PatientName,
                   opt => opt.MapFrom(src => src.Patient.User.FirstName + " " + src.Patient.User.LastName))
                   .ForMember(dest => dest.DoctorName,
                      opt => opt.MapFrom(src => src.Doctor.User.FirstName + " " + src.Doctor.User.LastName));

            CreateMap<MedicalRecordDTO, MedicalRecord>();

            // Billing mappings
            CreateMap<Billing, BillingDTO>();

            // User mappings
        }
    }
}
