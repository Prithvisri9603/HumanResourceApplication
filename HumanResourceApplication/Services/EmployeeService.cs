using System.Data;
using System.Numerics;
using AutoMapper;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using Microsoft.Data.SqlClient;
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
        public async Task AssignJob(string currentJobId, string newJobId)
        {
            // Find the employee matching the current JobId
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.JobId == currentJobId);

            if (employee == null)
            {
                throw new Exception($"No employee found with JobId '{currentJobId}'.");
            }

            // Update the JobId for the matched employee
            employee.JobId = newJobId;

            // Save changes to the database
            await _context.SaveChangesAsync();
        }


        public async Task AssignMan(decimal employeeId, decimal managerId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
                throw new Exception($"Employee with ID {employeeId} not found.");

            employee.ManagerId = managerId;

            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
        public async Task AssignDep(decimal employeeId, decimal departmentId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
                throw new Exception($"Employee with ID {employeeId} not found.");

            employee.DepartmentId = departmentId;

            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
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
            if (employee == null)
            {
                throw new Exception($"No employee found with the first name: {firstName}");
            }
            return _mapper.Map<EmployeeDTO>(employee);
        }

        public async Task<EmployeeDTO> FindByEmail(string email)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
            if (employee == null)
            {
                throw new Exception($"No employee found with the email: {email}");
            }
            return _mapper.Map<EmployeeDTO>(employee);
        }

        public async Task<EmployeeDTO> FindByPhoneNumber(string phone)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.PhoneNumber == phone);
            if (employee == null)
            {
                throw new Exception($"No employee found with the phone number: {phone}");
            }
            return _mapper.Map<EmployeeDTO>(employee);
        }

        public async Task<List<EmployeeDTO>> FindAllEmployeeWithNoCommission()
        {
            var employees = await _context.Employees.Where(e => e.CommissionPct == null).ToListAsync();
            if (!employees.Any())
            {
                throw new Exception("No employees found with no commission.");
            }
            return _mapper.Map<List<EmployeeDTO>>(employees);
        }

        public async Task<decimal> FindTotalCommissionIssuedToDepartment(decimal departmentId)
        {
            var departmentExists = await _context.Departments.AnyAsync(d => d.DepartmentId == departmentId);
            if (!departmentExists)
            {
                throw new Exception($"Department with ID {departmentId} does not exist.");
            }

            var totalCommission = await _context.Employees
                .Where(e => e.DepartmentId == departmentId && e.CommissionPct != null)
                .SumAsync(e => e.CommissionPct ?? 0);

            return totalCommission;
        }

        public async Task<List<EmployeeDTO>> ListAllEmployeesByDepartment(decimal departmentId)
        {
            var employees = await _context.Employees.Where(e => e.DepartmentId == departmentId).ToListAsync();
            if (!employees.Any())
            {
                throw new Exception($"No employees found for department ID {departmentId}.");
            }
            return _mapper.Map<List<EmployeeDTO>>(employees);
        }

        public async Task<List<EmployeeDTO>> ListAllManagerDetails()
        {
            var managers = await _context.Employees.Where(e => e.ManagerId != null).ToListAsync();
            if (!managers.Any())
            {
                throw new Exception("No manager details found.");
            }
            return _mapper.Map<List<EmployeeDTO>>(managers);
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

            if (!result.Any())
            {
                throw new Exception("No employee data found grouped by location.");
            }

            return result;
        }

        public async Task<(string JobTitle, decimal MaxSalary)> FindMaxSalaryOfJobByEmployeeId(decimal employeeId)
        {
            var sql = $"EXEC MaxSalarybyJobId @employeeId = {employeeId}";
            var results = await _context.Database.ExecuteSqlRawAsync(sql);
            using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var jobTitle = reader["job_title"] as string ?? string.Empty;
                var maxSalary = reader["max_salary"] != DBNull.Value ? Convert.ToDecimal(reader["max_salary"]) : 0;

                return (jobTitle, maxSalary);
            }
            throw new Exception($"No job found for employee ID {employeeId}");
        }


        public async Task UpdateEmployeeEmail(string email, EmployeeDTO employeeDto)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
            if (employee == null)
            {
                throw new Exception($"Employee with email {email} not found.");
            }

            _mapper.Map(employeeDto, employee);
            employee.Email = email;

            await _context.SaveChangesAsync();
        }

    }
}
