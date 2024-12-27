using System.Data;
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
        #region Add Employee

        /// <summary>
        /// Adds a new employee to the database.
        /// Ensures the email is unique before saving.
        /// </summary>
        /// <param name="employee">The employee data to be added.</param>
        /// <exception cref="Exception">Thrown if the email address already exists.</exception>
        public async Task AddEmployee(EmployeeDTO employee)
        {
            bool emailExists = await _context.Employees.AnyAsync(e => e.Email == employee.Email);
            if (emailExists)
            {
                throw new Exception("Email address already exists");
            }
            var employees = _mapper.Map<Employee>(employee);
            _context.Employees.Add(employees);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Modify Employee

        /// <summary>
        /// Modifies an existing employee's details.
        /// Ensures the email address is unique before saving.
        /// </summary>
        /// <param name="employeeId">The ID of the employee to modify.</param>
        /// <param name="employee">The updated employee data.</param>
        /// <exception cref="Exception">Thrown if the email address already exists or the employee does not exist.</exception>
        public async Task ModifyEmployee(int employeeId, EmployeeDTO employee)
        {
            var employeedata = await _context.Employees.FindAsync(employeeId);
            if (employeedata != null)
            {
                bool emailExists = await _context.Employees.AnyAsync(e => e.Email == employee.Email);
                if (emailExists)
                {
                    throw new Exception("Email address already exists");
                }
                _mapper.Map(employee, employeedata);
                _context.Entry(employeedata).State = EntityState.Modified;
            }
            else
            {
                throw new Exception($"Employee with ID {employeeId} not found");
            }
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Assign Job

        /// <summary>
        /// Assigns a new job to an employee by replacing their current job.
        /// </summary>
        /// <param name="currentJobId">The current job ID of the employee.</param>
        /// <param name="newJobId">The new job ID to assign.</param>
        /// <exception cref="Exception">Thrown if no employee is found with the current job ID.</exception>
        public async Task AssignJob(string currentJobId, string newJobId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.JobId == currentJobId);
            if (employee == null)
            {
                throw new Exception($"No employee found with JobId '{currentJobId}'.");
            }
            employee.JobId = newJobId;
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Assign Manager

        /// <summary>
        /// Assigns a manager to an employee.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="managerId">The ID of the manager to assign.</param>
        /// <exception cref="Exception">Thrown if the employee does not exist.</exception>
        public async Task AssignMan(decimal employeeId, decimal managerId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                throw new Exception($"Employee with ID {employeeId} not found.");
            }
            employee.ManagerId = managerId;
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Assign Department

        /// <summary>
        /// Assigns a department to an employee.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="departmentId">The ID of the department to assign.</param>
        /// <exception cref="Exception">Thrown if the employee does not exist.</exception>
        public async Task AssignDep(decimal employeeId, decimal departmentId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                throw new Exception($"Employee with ID {employeeId} not found.");
            }
            employee.DepartmentId = departmentId;
            _context.Entry(employee).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Update Commission for Department

        /// <summary>
        /// Updates the commission percentage for all employees in a specified department.
        /// </summary>
        /// <param name="departmentId">The department ID.</param>
        /// <param name="commissionPercentage">The new commission percentage.</param>
        /// <exception cref="Exception">Thrown if no employees are found in the specified department.</exception>
        public async Task UpdateCommissionForDepartment(decimal departmentId, decimal commissionPercentage)
        {
            var employees = await _context.Employees.Where(e => e.DepartmentId == departmentId).ToListAsync();
            if (!employees.Any())
            {
                throw new Exception($"No employees found in Department ID {departmentId}");
            }
            foreach (var employee in employees)
            {
                employee.CommissionPct = commissionPercentage;
            }
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Find Employee by First Name

        /// <summary>
        /// Finds an employee by their first name.
        /// </summary>
        /// <param name="firstName">The first name of the employee.</param>
        /// <returns>The employee's details.</returns>
        /// <exception cref="Exception">Thrown if no employee is found with the given first name.</exception>
        public async Task<EmployeeDTO> FindByFirstName(string firstName)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.FirstName == firstName);
            if (employee == null)
            {
                throw new Exception($"No employee found with the first name: {firstName}");
            }
            return _mapper.Map<EmployeeDTO>(employee);
        }

        #endregion

        #region Find Employee by Email

        /// <summary>
        /// Finds an employee by their email.
        /// </summary>
        /// <param name="email">The email of the employee.</param>
        /// <returns>The employee's details.</returns>
        /// <exception cref="Exception">Thrown if no employee is found with the given email.</exception>
        public async Task<EmployeeDTO> FindByEmail(string email)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == email);
            if (employee == null)
            {
                throw new Exception($"No employee found with the email: {email}");
            }
            return _mapper.Map<EmployeeDTO>(employee);
        }

        #endregion

        #region Find Employee by Phone Number

        /// <summary>
        /// Finds an employee by their phone number.
        /// </summary>
        /// <param name="phone">The phone number of the employee.</param>
        /// <returns>The employee's details.</returns>
        /// <exception cref="Exception">Thrown if no employee is found with the given phone number.</exception>
        public async Task<EmployeeDTO> FindByPhoneNumber(string phone)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.PhoneNumber == phone);
            if (employee == null)
            {
                throw new Exception($"No employee found with the phone number: {phone}");
            }
            return _mapper.Map<EmployeeDTO>(employee);
        }

        #endregion

        #region List Employees with No Commission

        /// <summary>
        /// Lists all employees who do not have a commission assigned.
        /// </summary>
        /// <returns>A list of employees with no commission.</returns>
        /// <exception cref="Exception">Thrown if no employees are found with no commission.</exception>
        public async Task<List<EmployeeDTO>> FindAllEmployeeWithNoCommission()
        {
            var employees = await _context.Employees.Where(e => e.CommissionPct == null).ToListAsync();
            if (!employees.Any())
            {
                throw new Exception("No employees found with no commission.");
            }
            return _mapper.Map<List<EmployeeDTO>>(employees);
        }

        #endregion

        #region Find Total Commission Issued to Department

        /// <summary>
        /// Finds the total commission issued to a specific department.
        /// </summary>
        /// <param name="departmentId">The ID of the department.</param>
        /// <returns>The total commission issued to the department.</returns>
        /// <exception cref="Exception">Thrown if the department does not exist or has no commission data.</exception>
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

        #endregion

        #region List All Employees by Department

        /// <summary>
        /// Lists all employees in a specific department.
        /// </summary>
        /// <param name="departmentId">The department ID.</param>
        /// <returns>A list of employees in the department.</returns>
        /// <exception cref="Exception">Thrown if no employees are found in the specified department.</exception>
        public async Task<List<EmployeeDTO>> ListAllEmployeesByDepartment(decimal departmentId)
        {
            var employees = await _context.Employees.Where(e => e.DepartmentId == departmentId).ToListAsync();
            if (!employees.Any())
            {
                throw new Exception($"No employees found for department ID {departmentId}.");
            }
            return _mapper.Map<List<EmployeeDTO>>(employees);
        }

        #endregion

        #region List All Manager Details

        /// <summary>
        /// Lists details of all employees who are managers.
        /// </summary>
        /// <returns>A list of managers.</returns>
        /// <exception cref="Exception">Thrown if no managers are found.</exception>
        public async Task<List<EmployeeDTO>> ListAllManagerDetails()
        {
            var managers = await _context.Employees.Where(e => e.ManagerId != null).ToListAsync();
            if (!managers.Any())
            {
                throw new Exception("No manager details found.");
            }
            return _mapper.Map<List<EmployeeDTO>>(managers);
        }

        #endregion

        #region Count Employees Grouped by Location

        /// <summary>
        /// Counts all employees grouped by their location.
        /// </summary>
        /// <returns>A dictionary with location IDs as keys and employee counts as values.</returns>
        /// <exception cref="Exception">Thrown if no employees are found grouped by location.</exception>
        public async Task<Dictionary<string, int>> CountAllEmployeesGroupByLocation()
        {
            var result = await _context.Employees.Join(
                _context.Departments,
                employee => employee.DepartmentId,
                department => department.DepartmentId,
                (employee, department) => new { department.LocationId, employee.EmployeeId }
            )
            .Join(
                _context.Locations,
                location => location.LocationId,
                loc => loc.LocationId,
                (location, loc) => new { loc.City, location.EmployeeId }
            )
            .GroupBy(x => x.City)
            .Select(g => new { Location = g.Key, EmployeeCount = g.Count() })
            .ToDictionaryAsync(g => g.Location, g => g.EmployeeCount);

            if (!result.Any())
            {
                throw new Exception("No employee data found grouped by location.");
            }

            return result;
        }

        #endregion

        #region Find Maximum Salary of Job by Employee ID

        /// <summary>
        /// Finds the maximum salary and job description for a job by employee ID.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <returns>A tuple containing the job description and the maximum salary.</returns>
        /// <exception cref="Exception">Thrown if no job is found for the specified employee ID.</exception>
        public async Task<(string JobDescription, decimal MaxSalary)> FindMaxSalaryOfJobByEmployeeId(decimal employeeId)
        {
            var sql = $"EXEC MaxSalarybyJobId @employeeId = {employeeId}";
            var connection = _context.Database.GetDbConnection();

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

        #endregion

        #region Update Employee Email

        /// <summary>
        /// Updates the email address of an employee.
        /// </summary>
        /// <param name="email">The current email of the employee.</param>
        /// <param name="employeeDto">The updated employee data.</param>
        /// <exception cref="Exception">Thrown if the employee does not exist.</exception>
        public async Task UpdateEmployeeEmail(string currentemail, string newemail)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == currentemail);

            if (employee == null)
            {
                throw new Exception($"No employee found with JobId '{currentemail}'.");
            }


            employee.Email = newemail;


            await _context.SaveChangesAsync();
        }

        #endregion

    }
}
