using HospitalManagement.DTOs;

namespace HospitalManagement.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDTO>> GetAllDepartmentsAsync();
        Task<DepartmentDTO> GetDepartmentByIdAsync(int departmentId);
        Task<DepartmentDTO> CreateDepartmentAsync(DepartmentDTO departmentDTO);
        Task<DepartmentDTO> UpdateDepartmentAsync(int departmentId, DepartmentDTO departmentDTO);
        Task<bool> DeleteDepartmentAsync(int departmentId);
    }
}
