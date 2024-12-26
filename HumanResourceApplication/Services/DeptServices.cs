using AutoMapper;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceApplication.Services
{
    public class DeptServices : IDepartmentRepository
    {
        private readonly HrContext _hrContext;
        private readonly IMapper _mapper;

        #region constructor injection
        public DeptServices(HrContext hrContext, IMapper mapper)
        {
            _hrContext = hrContext;
            _mapper = mapper;

        }
        #endregion

        #region GetDepartment
        /// <summary>
        /// Lsiting all the dept data
        /// </summary>
        /// <returns></returns>

        public async Task<List<DepartmentDTO>> GetDepartment()
        {
            var departmentlist = await _hrContext.Departments.ToListAsync();
            var departmentDTOList = _mapper.Map<List<DepartmentDTO>>(departmentlist);
            return departmentDTOList;

        }
        #endregion

        public async Task<Department> GetDepartmentByName(string name)
        {
            return await _hrContext.Departments
                .FirstOrDefaultAsync(d => d.DepartmentName == name);
        }

        public async Task<Department> GetDepartmentById(decimal departmentId)
        {
            // Assuming you're using Entity Framework Core or similar ORM to fetch the department by ID
            return await _hrContext.Departments
                .Where(d => d.DepartmentId == departmentId)
                .FirstOrDefaultAsync();
        }

        #region  AddDepartment
        /// <summary>
        /// Input:Enter all the department details to be added
        /// </summary>
        /// <param name="departmentdto"></param>
        /// <returns></returns>
        public async Task AddDepartment(DepartmentDTO departmentdto)
        {
            var addDepartment = _mapper.Map<Department>(departmentdto);
            _hrContext.Add(addDepartment);
            await _hrContext.SaveChangesAsync();
        }
        #endregion

        #region UpdateDepartment
        /// <summary>
        /// Input:Enter the departmentId as decimal
        /// </summary>
        /// <param name="departmentId"></param>
        /// <param name="departmentdto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpdateDepartment(decimal departmentId, DepartmentDTO departmentdto)
        {
            var findId = await _hrContext.Departments.FindAsync(departmentId);
            var deptUpt = _mapper.Map<Department>(findId);
            if (deptUpt != null)
            {

                _hrContext.Entry(deptUpt).CurrentValues.SetValues(departmentdto);

                await _hrContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Department Id is not found");
            }

        }
        #endregion

        #region GetAllDeptDetailsForEmp
        /// <summary>
        /// Input:Enter the input as EmpId in decimal
        /// </summary>
        /// <param name="empId"></param>
        /// <returns></returns>
        public async Task<List<DepartmentDTO>> GetAllDeptDetailsForEmp(decimal empId)
        {
            var Departmentdetails = await _hrContext.Departments
                .Join(
                _hrContext.Employees,
                dept => dept.DepartmentId,
                emp => emp.DepartmentId,
                (dept, emp) => new { dept, emp })
                .Where(j => j.emp.EmployeeId == empId).Select(j => new DepartmentDTO
                {
                    DepartmentId = j.dept.DepartmentId,
                    DepartmentName = j.dept.DepartmentName,
                    ManagerId = j.dept.ManagerId,
                    LocationId = j.dept.LocationId

                }).ToListAsync();
            return Departmentdetails;
        }

        #endregion


        #region GetMaximumSalary
        /// <summary>
        /// Getting the maximum salary based on the department id in the employee table
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, decimal?>> GetMaximumSalary(decimal departmentId)
        {

            var result = await _hrContext.Departments
                .Where(d => d.DepartmentId == departmentId)
                .Join(
                    _hrContext.Employees,
                    dept => dept.DepartmentId,
                    emp => emp.DepartmentId,
                    (dept, emp) => new { dept.DepartmentName, emp.Salary }
                )
                .GroupBy(x => x.DepartmentName)
                .ToDictionaryAsync(
                    g => g.Key,               // Key: DepartmentName
                    g => g.Max(x => x.Salary) // Value: Max Salary in that department
                );
            return result;
        }
        #endregion

        #region GetMinSalary
        /// <summary>
        /// Getting the maximum salary based on the department id in the employee table
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, decimal?>> GetMinSalary(decimal departmentId)
        {

            var result = await _hrContext.Departments
                .Where(d => d.DepartmentId == departmentId)
                .Join(
                    _hrContext.Employees,
                    dept => dept.DepartmentId,
                    emp => emp.DepartmentId,
                    (dept, emp) => new { dept.DepartmentName, emp.Salary }
                )
                .GroupBy(x => x.DepartmentName)
                .ToDictionaryAsync(
                    g => g.Key,               // Key: DepartmentName
                    g => g.Min(x => x.Salary) // Value: Min Salary in that department
                );
            return result;


        }
        #endregion



        #region DeleteDepartmentById

        /// <summary>
        /// Delete by department id
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns></returns>

        public async Task DeleteDepartmentById(decimal departmentId)
        {
            var departmentToDelete = await _hrContext.Departments.FindAsync(departmentId);
            if (departmentToDelete != null)
            {
                _hrContext.Departments.Remove(departmentToDelete);
                await _hrContext.SaveChangesAsync();
            }
        }
        #endregion


    }
}


    

