using HospitalManagement.DTOs;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace HospitalManagement.Services
{
    public interface IAuthService
    {
        Task<LoginResponseModel> Login(LoginModel model);
        Task<IdentityResult> RegisterPatient(PatientRegistrationDTO model);
        Task<IdentityResult> RegisterDoctor(DoctorRegistrationDTO model);
        Task<IdentityResult> RegisterAdmin(string email, string password,string FullName);
        Task<IdentityResult> RegisterReceptionist(string email, string password, string FullName);
    }
}
