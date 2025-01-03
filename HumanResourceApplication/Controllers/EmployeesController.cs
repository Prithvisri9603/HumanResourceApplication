using FluentValidation;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;
using HumanResourceApplication.Utility;
using HumanResourceApplication.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace HumanResourceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepo _employeeRepo;
        private readonly IValidator<EmployeeDTO> _employeeValidator;

        private readonly IConfiguration _configuration;
        public EmployeesController(IEmployeeRepo employeeRepo, IValidator<EmployeeDTO> employeevalidator, IConfiguration configuration)
        {
            _employeeRepo = employeeRepo;
            _employeeValidator = employeevalidator;
            _configuration = configuration;
        }

 #region AddEmployee Method

        /// <summary>
        /// Adds a new employee.
        /// </summary>
        /// <param name="employee">The employee data transfer object.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("Add new Employee")]
        public async Task<IActionResult> AddEmployee(EmployeeDTO employee)
        {
            var validationResult = await _employeeValidator.ValidateAsync(employee);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    timeStamp = DateTime.UtcNow,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }
            try
            {
                await _employeeRepo.AddEmployee(employee);
                return Ok("Record Created Successfully");
                //Helper.PublishToEventGrid(_configuration, employee);
                //return Ok("Event Triggered");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("email address already exists"))
                    return BadRequest(new { timestamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region ModifyEmployee Method

        /// <summary>
        /// Modifies an existing employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <param name="employee">The updated employee data transfer object.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("Modify")]
        public async Task<IActionResult> ModifyEmployee([FromQuery] int employeeId, [FromBody] EmployeeDTO employee)
        {
            var validationResult = await _employeeValidator.ValidateAsync(employee);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    timeStamp = DateTime.UtcNow,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            try
            {
                decimal empId = Convert.ToDecimal(employeeId);
                //await _employeeRepo.ModifyEmployee(empId, employee);
                Helper.PublishToEventGrid(_configuration, employee);
                return Ok("Event Triggered");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("email address already exists"))
                    return BadRequest(new { timestamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region AssignJob Method

        /// <summary>
        /// Assigns a new job to an employee.
        /// </summary>
        /// <param name="currentJobId">The current job ID.</param>
        /// <param name="newJobId">The new job ID.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin,HR Team")]
        [HttpPut("Assign Job")]
        public async Task<IActionResult> AssignJob([FromQuery] string currentJobId, [FromQuery] string newJobId)
        {
            try
            {
                await _employeeRepo.AssignJob(currentJobId, newJobId);
                return Ok("Record Modified Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region AssignManager Method

        /// <summary>
        /// Assigns a manager to an employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <param name="managerId">The manager ID.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin,HR Team")]
        [HttpPut("Assign Manager")]
        public async Task<IActionResult> AssignMan(decimal employeeId, decimal managerId)
        {
            try
            {
                await _employeeRepo.AssignMan(employeeId, managerId);
                return Ok("Record Modified Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region AssignDepartment Method

        /// <summary>
        /// Assigns a department to an employee.
        /// </summary>
        /// <param name="employeeId">The employee ID.</param>
        /// <param name="departmentId">The department ID.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpPut("Assign Department")]
        public async Task<IActionResult> AssignDep([FromQuery] decimal employeeId, [FromQuery] decimal departmentId)
        {
            try
            {
                await _employeeRepo.AssignDep(employeeId, departmentId);
                return Ok("Record Modified Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region UpdateCommissionForDepartment Method

        /// <summary>
        /// Updates the commission percentage for employees in the sales department.
        /// </summary>
        /// <param name="departmentId">The ID of the sales department.</param>
        /// <param name="commissionPercentage">The new commission percentage to be applied.</param>
        /// <returns>Returns a success message if the operation is successful, otherwise a Bad Request response with the error details.</returns>
        [Authorize(Roles = "Admin,HR Team")]
        [HttpPut("update Commission for sales department")]
        public async Task<IActionResult> UpdateCommissionForDepartment([FromQuery] decimal departmentId, [FromQuery] decimal commissionPercentage)
        {
            try
            {
                await _employeeRepo.UpdateCommissionForDepartment(departmentId, commissionPercentage);
                return Ok("Record Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region FindByFirstName Method

        /// <summary>
        /// Finds an employee by their first name.
        /// </summary>
        /// <param name="firstName">The first name of the employee.</param>
        /// <returns>An IActionResult containing the employee data.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpGet("find by first name")]
        public async Task<IActionResult> FindByFirstName(string firstName)
        {
            try
            {
                var employee = await _employeeRepo.FindByFirstName(firstName);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region FindByEmail Method

        /// <summary>
        /// Finds an employee by their email address.
        /// </summary>
        /// <param name="email">The email address of the employee.</param>
        /// <returns>An IActionResult containing the employee data.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpGet("findemail")]
        public async Task<IActionResult> FindByEmail(string email)
        {
            try
            {
                var employee = await _employeeRepo.FindByEmail(email);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region FindByPhoneNumber Method

        /// <summary>
        /// Finds an employee by their phone number.
        /// </summary>
        /// <param name="phone">The phone number of the employee.</param>
        /// <returns>An IActionResult containing the employee data.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpGet("find phone")]
        public async Task<IActionResult> FindByPhoneNumber(string phone)
        {
            try
            {
                var employee = await _employeeRepo.FindByPhoneNumber(phone);
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region FindAllEmployeeWithNoCommission Method

        /// <summary>
        /// Finds all employees with no commission.
        /// </summary>
        /// <returns>An IActionResult containing the list of employees with no commission.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpGet("find All Employee With No Commission")]
        public async Task<IActionResult> FindAllEmployeeWithNoCommission()
        {
            try
            {
                var employees = await _employeeRepo.FindAllEmployeeWithNoCommission();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region FindTotalCommissionIssuedToDepartment Method

        /// <summary>
        /// Finds the total commission issued to a department.
        /// </summary>
        /// <param name="departmentId">The department ID.</param>
        /// <returns>An IActionResult containing the total commission issued to the department.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpGet("find Total Commission Issued To Employee")]
        public async Task<IActionResult> FindTotalCommissionIssuedToDepartment(decimal departmentId)
        {
            try
            {
                var totalCommission = await _employeeRepo.FindTotalCommissionIssuedToDepartment(departmentId);
                return Ok(new { departmentId, sum = totalCommission });
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region ListAllEmployeesByDepartment Method

        /// <summary>
        /// Lists all employees by department.
        /// </summary>
        /// <param name="departmentId">The department ID.</param>
        /// <returns>An IActionResult containing the list of employees in the department.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpGet("list All Employees by department")]
        public async Task<IActionResult> ListAllEmployeesByDepartment(decimal departmentId)
        {
            try
            {
                var employees = await _employeeRepo.ListAllEmployeesByDepartment(departmentId);
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region ListAllManagerDetails Method

        /// <summary>
        /// Lists all manager details.
        /// </summary>
        /// <returns>An IActionResult containing the list of managers.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpGet("list All Manager Details")]
        public async Task<IActionResult> ListAllManagerDetails()
        {
            try
            {
                var managers = await _employeeRepo.ListAllManagerDetails();
                return Ok(managers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region CountAllEmployeesGroupByLocation Method

        /// <summary>
        /// Counts all employees grouped by their respective office locations.
        /// </summary>
        /// <returns>Returns the employee count grouped by location or a Bad Request response if an error occurs.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpGet("location wise count of employees")]
        public async Task<IActionResult> CountAllEmployeesGroupByLocation()
        {
            try
            {
                var result = await _employeeRepo.CountAllEmployeesGroupByLocation();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region FindMaxSalaryOfJobByEmployeeId Method

        /// <summary>
        /// Finds the maximum salary of a job by employee ID.
        /// </summary>
        /// <param name="empid">The employee ID.</param>
        /// <returns>An IActionResult containing the job title and maximum salary.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpGet("find max salary of job")]
        public async Task<IActionResult> FindMaxSalaryOfJobByEmployeeId(decimal empid)
        {
            try
            {
                var result = await _employeeRepo.FindMaxSalaryOfJobByEmployeeId(empid);
                return Ok(new { JobTitle = result.JobDescription, MaxSalary = result.MaxSalary });
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region UpdateEmployeeEmail Method

        /// <summary>
        /// Updates the email address of an employee.
        /// </summary>
        /// <param name="currentemail">The current email address.</param>
        /// <param name="newemail">The new email address.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [Authorize(Roles = "Admin,HR Team")]
        [HttpPut("Update Email")]
        public async Task<IActionResult> UpdateEmployeeEmail(string currentemail, string newemail)
        {
            try
            {
                await _employeeRepo.UpdateEmployeeEmail(currentemail, newemail);
                return Ok("Record Modified Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion
    }
}

