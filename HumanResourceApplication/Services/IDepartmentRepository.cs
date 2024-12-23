using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;

namespace HumanResourceApplication.Services
{
    public interface IDepartmentRepository
    {
       //Add new department
        Task AddDepartment(DepartmentDTO department);

        //get all
        Task<List<DepartmentDTO>> GetDepartment();

        //update department
        Task UpdateDepartment(decimal departmentId, DepartmentDTO department);

        //GetMaxSalary
        Task<Dictionary<string,decimal?>> GetMaximumSalary(decimal departmentId);

        //GetMinSalary

        Task<Dictionary<string, decimal?>> GetMinSalary(decimal departmentId);

        //Get all department
        Task<List<DepartmentDTO>> GetAllDeptDetailsForEmp(decimal empId);

        //delete by id
        Task DeleteDepartmentById(decimal departmentId);


    }
}
