using HospitalManagement.DTOs;
using HospitalManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(Roles = "Admin")] 
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: api/Department
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            return Ok(departments);
        }

        // GET: api/Department/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var department = await _departmentService.GetDepartmentByIdAsync(id);
            if (department == null)
                return NotFound(new { message = "Department not found." });

            return Ok(department);
        }

        // POST: api/Department
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentDTO departmentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _departmentService.CreateDepartmentAsync(departmentDTO);
            return CreatedAtAction(nameof(GetDepartmentById), new { id = created.Id }, created);
        }

        // PUT: api/Department/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] DepartmentDTO departmentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _departmentService.UpdateDepartmentAsync(id, departmentDTO);
            if (updated == null)
                return NotFound(new { message = "Department not found." });

            return Ok(updated);
        }

        // DELETE: api/Department/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var deleted = await _departmentService.DeleteDepartmentAsync(id);
            if (!deleted)
                return BadRequest(new { message = "Cannot delete department. It may not exist or has assigned doctors." });

            return Ok(new { message = "Department deleted successfully." });
        }
    }
}
