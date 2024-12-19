using System.Numerics;
using AutoMapper;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceApplication.Services
{
    public class EmployeeService : IEmployeeRepo
    {
        private readonly HrContext _context;
        private readonly IMapper _mapper;
        public EmployeeService(HrContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task AddEmployee(EmployeeDTO employee)
        {
            var employees = _mapper.Map<Employee>(employee);
            _context.Employees.Add(employees);
            await _context.SaveChangesAsync();
            //  return "Record Modified Successfully";
        }
        public async Task ModifyEmployee(int employeeId, EmployeeDTO employee)
        {
            var employeedata = await _context.Employees.FindAsync(employeeId);
            if (employeedata != null)
            {
                _mapper.Map(employee, employeedata);
                _context.Entry(employeedata).State = EntityState.Modified;
                //return "Record Modified Successfully";
            }
            await _context.SaveChangesAsync();
            // return "Employee id not found";
        }
        public async Task AssignJob(string JobId, EmployeeDTO employee)
        {
            var employeedata = await _context.Employees.FindAsync(JobId);
            //if (employeedata == null)
            //{
            //   // return "Job Id Invalid";
            //}

            _mapper.Map(employee, employeedata);

            // Update the JobId in the Employee entity
            employeedata.JobId = JobId;

            // Save changes to the database
            await _context.SaveChangesAsync();

            //return "Record Modified Successfully";
        }
        public async Task AssignMan(decimal ManagerId, EmployeeDTO employee)
        {
            var employeeEntity = await _context.Employees.FindAsync(ManagerId);
            //if (employeeEntity == null)
            //{
            //    return "Manager Not found";
            //}
            // Map the EmployeeDTO to the Employee entity
            _mapper.Map(employee, employeeEntity);

            // Assign the managerId to the ManagerId field
            employeeEntity.ManagerId = ManagerId;

            // Save changes to the database
            await _context.SaveChangesAsync();

            // return "Record Modified Successfully";
        }
        public async Task AssignDep(decimal DepartmentId, EmployeeDTO employee)
        {
            var employeedata = await _context.Employees.FindAsync(DepartmentId);
            if (employeedata != null)
            {
                _context.Entry(employee).CurrentValues.SetValues(employeedata);
            }
            await _context.SaveChangesAsync();
            //_mapper.Map(employee, employeedata);
            //employeedata.DepartmentId = DepartmentId;
            //await _context.SaveChangesAsync();
            //return "Record Modified Successfully";
        }
        public async Task UpdateCommissionForDepartment(decimal departmentId, decimal commissionPercentage)
        {
            var employees = await _context.Employees
        .Where(e => e.DepartmentId == departmentId)
        .ToListAsync();

            if (employees.Any())
            {
                foreach (var employee in employees)
                {
                    employee.CommissionPct = commissionPercentage;
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception($"No employees found in Department ID {departmentId}");
            }
        
        }
        public async Task<EmployeeDTO> FindByFirstName(string firstName)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.FirstName == firstName);
            return _mapper.Map<EmployeeDTO>(employee);
        }
        public async Task<EmployeeDTO> FindByEmail(string email)
        {
            var employee= await _context.Employees.FirstOrDefaultAsync(e =>e.Email == email);
            return _mapper.Map<EmployeeDTO>(employee);
        }
        public async Task<EmployeeDTO> FindByPhoneNumber(string phone)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.PhoneNumber==phone);
            return _mapper.Map<EmployeeDTO>(employee);
        }
        public async Task<List<EmployeeDTO>> FindAllEmployeeWithNoCommission()
        {
            var employee = await _context.Employees.Where(e => e.CommissionPct==null).ToListAsync();
            return _mapper.Map<List<EmployeeDTO>>(employee);
        }
        public async Task<decimal> FindTotalCommissionIssuedToDepartment(decimal departmentId)
        {
            return await _context.Employees
           .Where(e => e.DepartmentId == departmentId && e.CommissionPct != null)
           .SumAsync(e => e.CommissionPct ?? 0);
        }
        public async Task<List<EmployeeDTO>> ListAllEmployeesByDepartment(decimal departmentId)
        {
            var employee = await _context.Employees.Where(e => e.DepartmentId==departmentId).ToListAsync();
            return _mapper.Map<List<EmployeeDTO>>(employee);
        }
        public async Task<List<EmployeeDTO>> ListAllManagerDetails()
        {
            var employee=await _context.Employees.Where(e=>e.ManagerId!=null).ToListAsync();
            return _mapper.Map<List<EmployeeDTO>>(employee);
        }
        public async Task<Dictionary<decimal, int>> CountAllEmployeesGroupByLocation()
        {
            var result = await _context.Employees.Join(
                 _context.Departments, 
                 employee => employee.DepartmentId, 
                 department => department.DepartmentId,
                 (employee, department) => new { department.LocationId, employee.EmployeeId }
             )
             .GroupBy(x => x.LocationId) 
             .Select(g => new { LocationId = g.Key ?? 0, EmployeeCount = g.Count() })
             .ToDictionaryAsync(g => g.LocationId, g => g.EmployeeCount);

            return result;
        }
        public async Task<(string JobDescription, decimal MaxSalary)> FindMaxSalaryOfJobByEmployeeId(decimal employeeId)
        {
            // Find the employee by ID
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                throw new Exception("Employee not found");
            }

            // Get the JobId of the employee
            var jobId = employee.JobId;

            // Find the job description for the given JobId
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == jobId);
            if (job == null)
            {
                throw new Exception("Job not found");
            }

            // Find the maximum salary for employees with the same JobId
            var maxSalary = await _context.Employees
                .Where(e => e.JobId == jobId)
                .MaxAsync(e => e.Salary ?? 0);

            // Return the JobDescription and MaxSalary
            return (job.JobTitle, maxSalary);
        }
        public async Task UpdateEmployeeEmail(string email, EmployeeDTO employee)
        {
            var employees = await _context.Employees.FirstOrDefaultAsync(e => e.Email==email);
            if (employee != null)
            {
                _mapper.Map(employee, employees);
                employee.Email = email;

                await _context.SaveChangesAsync();
            }
        }
    }
}
