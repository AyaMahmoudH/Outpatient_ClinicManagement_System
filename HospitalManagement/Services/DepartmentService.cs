using AutoMapper;
using HospitalManagement.Data;
using HospitalManagement.DTOs;
using HospitalManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Services
{
    public class DepartmentService:IDepartmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILoggingService _loggingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public DepartmentService(
            ApplicationDbContext context,
            ILoggingService loggingService,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _context = context;
            _loggingService = loggingService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _mapper = mapper;

        }

        public async Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync()
        {
            var departments = await _context.Departments.ToListAsync();
            return _mapper.Map<IEnumerable<DepartmentDTO>>(departments);
        }

        public async Task<DepartmentDTO> GetDepartmentByIdAsync(int departmentId)
        {
            var department = await _context.Departments.FindAsync(departmentId);
            return department != null ? _mapper.Map<DepartmentDTO>(department) : null;
        }

        public async Task<DepartmentDTO> CreateDepartmentAsync(DepartmentDTO departmentDTO)
        {
            var department = new Department
            {
                Name = departmentDTO.Name,
                Description = departmentDTO.Description,
                Location = departmentDTO.Location
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            var user = await GetCurrentUser();
            await _loggingService.LogActivityAsync(
                user?.Id ?? "System",
                user?.UserName ?? "System",
                "Admin",
                "Create",
                "Department",
                department.Id.ToString(),
                "Department created",
                _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "");

            return _mapper.Map<DepartmentDTO>(department);
        }

        public async Task<DepartmentDTO> UpdateDepartmentAsync(int departmentId, DepartmentDTO departmentDTO)
        {
            var department = await _context.Departments.FindAsync(departmentId);
            if (department == null)
            {
                return null;
            }

            department.Name = departmentDTO.Name;
            department.Description = departmentDTO.Description;
            department.Location = departmentDTO.Location;

            await _context.SaveChangesAsync();

            var user = await GetCurrentUser();
            await _loggingService.LogActivityAsync(
                user?.Id ?? "System",
                user?.UserName ?? "System",
                "Admin",
                "Update",
                "Department",
                department.Id.ToString(),
                "Department updated",
                _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "");

            return _mapper.Map<DepartmentDTO>(department);
        }

        public async Task<bool> DeleteDepartmentAsync(int departmentId)
        {
            var department = await _context.Departments.FindAsync(departmentId);
            if (department == null)
            {
                return false;
            }

            // Check if there are doctors assigned to this department
            bool hasDoctors = await _context.Doctors.AnyAsync(d => d.DepartmentId == departmentId);
            if (hasDoctors)
            {
                return false; // Cannot delete a department with assigned doctors
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            var user = await GetCurrentUser();
            await _loggingService.LogActivityAsync(
                user?.Id ?? "System",
                user?.UserName ?? "System",
                "Admin",
                "Delete",
                "Department",
                departmentId.ToString(),
                "Department deleted",
                _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "");

            return true;
        }

        
        private async Task<ApplicationUser> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return user;
        }
    }
}
