using HospitalManagement.DTOs;
using HospitalManagement.Models;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HospitalManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.Login(model);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid credentials or inactive account" });

            return Ok(result);
        }

        [HttpPost("register-patient")]
        public async Task<IActionResult> RegisterPatient([FromBody] PatientRegistrationDTO model)
        {
            var result = await _authService.RegisterPatient(model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Patient registered successfully" });
        }

        [HttpPost("register-doctor")]
        public async Task<IActionResult> RegisterDoctor([FromBody] DoctorRegistrationDTO model)
        {
            var result = await _authService.RegisterDoctor(model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Doctor registered successfully" });
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterAdminDto model)
        {
            var result = await _authService.RegisterAdmin(model.Email, model.Password,model.FullName);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Admin registered successfully" });
        }

        [HttpPost("register-receptionist")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterReceptionist([FromBody] RegisterReceptionistDto model)
        {
            var result = await _authService.RegisterReceptionist(model.Email, model.Password,model.FullName);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Receptionist registered successfully" });
        }
    }

    public class RegisterAdminDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }

    }

    public class RegisterReceptionistDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }
}