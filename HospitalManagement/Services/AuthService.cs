using HospitalManagement.DTOs;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HospitalManagement.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly ILoggingService _loggingService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IPatientService patientService,
            IDoctorService doctorService,
            ILoggingService loggingService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _patientService = patientService;
            _doctorService = doctorService;
            _loggingService = loggingService;
        }
        public async Task<LoginResponseModel> Login(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new LoginResponseModel { Succeeded = false };
            }

            if (!user.IsActive)
            {
                return new LoginResponseModel { Succeeded = false };
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var tokenExpiration = DateTime.Now.AddHours(3);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: tokenExpiration,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            await _loggingService.LogActivityAsync(
                user.Id,
                user.UserName,
                string.Join(", ", userRoles),
                "Login",
                "User",
                user.Id,
                "User login successful",
                "");

            return new LoginResponseModel
            {
                Succeeded = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = userRoles.ToList(),
                Expiration = tokenExpiration
            };
        }
        public async Task<IdentityResult> RegisterPatient(PatientRegistrationDTO model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User already exists!" });
            }

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email,
                FullName = model.FullName ?? $"{model.FirstName} {model.LastName}",
                PhoneNumber = model.PhoneNumber,
                DateCreated = DateTime.Now,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return result;
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Patient))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Patient));
            }

            await _userManager.AddToRoleAsync(user, UserRoles.Patient);

            // Create patient profile
            var patientDTO = new PatientDTO
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                FullName = model.FullName ?? $"{model.FirstName} {model.LastName}",
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmergencyContact = model.EmergencyContact,
                EmergencyContactPhone = model.EmergencyContactPhone,
                BloodType = model.BloodType,
                Allergies = model.Allergies,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                InsuranceInformation = model.InsuranceInformation
            };

            // Create patient profile
            await _patientService.CreatePatientAsync(patientDTO);

            await _loggingService.LogActivityAsync(
                user.Id,
                user.UserName,
                UserRoles.Patient,
                "Register",
                "User",
                user.Id,
                "Patient registration successful",
                "");

            return result;
        }
        public async Task<IdentityResult> RegisterDoctor(DoctorRegistrationDTO model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User already exists!" });
            }

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                FullName = model.FullName ?? $"{model.FirstName} {model.LastName}",
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateCreated = DateTime.Now,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return result;
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Doctor))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Doctor));
            }

            await _userManager.AddToRoleAsync(user, UserRoles.Doctor);

            // Create doctor profile
            var doctorDTO = new DoctorDTO
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                FullName = model.FullName ?? $"{model.FirstName} {model.LastName}",
                PhoneNumber = model.PhoneNumber,
                DepartmentId = model.DepartmentId,
                Specialization = model.Specialization,
                Qualifications = model.Qualifications,
                LicenseNumber = model.LicenseNumber,
                IsAvailable = true
            };

            // Create doctor profile
            await _doctorService.CreateDoctorAsync(doctorDTO);

            await _loggingService.LogActivityAsync(
                user.Id,
                user.UserName,
                UserRoles.Doctor,
                "Register",
                "User",
                user.Id,
                "Doctor registration successful",
                "");

            return result;
        }

        public async Task<IdentityResult> RegisterAdmin(string email, string password,string FullName)
        {
            var userExists = await _userManager.FindByEmailAsync(email);
            if (userExists != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User already exists!" });
            }

            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FirstName = "Admin",
                LastName = "User",
                FullName = FullName,
                DateCreated = DateTime.Now,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return result;
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            }

            await _userManager.AddToRoleAsync(user, UserRoles.Admin);

            await _loggingService.LogActivityAsync(
                user.Id,
                user.UserName,
                UserRoles.Admin,
                "Register",
                "User",
                user.Id,
                "Admin registration successful",
                "");

            return result;
        }
        public async Task<IdentityResult> RegisterReceptionist(string email, string password,string fullName)
        {
            var userExists = await _userManager.FindByEmailAsync(email);
            if (userExists != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User already exists!" });
            }

            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FullName = fullName,
                FirstName = "Receptionist",
                LastName = "User",
                DateCreated = DateTime.Now,
                IsActive = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return result;
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.Receptionist))
            {
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Receptionist));
            }

            await _userManager.AddToRoleAsync(user, UserRoles.Receptionist);

            await _loggingService.LogActivityAsync(
                user.Id,
                user.UserName,
                UserRoles.Receptionist,
                "Register",
                "User",
                user.Id,
                "Receptionist registration successful",
                "");

            return result;
        }
    }
}

