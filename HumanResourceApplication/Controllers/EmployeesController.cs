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
        public EmployeesController(IEmployeeRepo employeeRepo, IValidator<EmployeeDTO> employeevalidator)
        {
            _employeeRepo = employeeRepo;
            _employeeValidator = employeevalidator;
        }
        #region Add Employee

        /// <summary>
        /// Adds a new employee record to the system. 
        /// This endpoint validates the provided employee data and is accessible only to users with the Admin role.
        /// </summary>
        /// <param name="employee">The employee data to be added.</param>
        /// <returns>Returns a success message if the operation is successful, otherwise a Bad Request response with the validation or error details.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("Add new Employee")]
        public async Task<IActionResult> AddEmployee(EmployeeDTO employee)
        {
            var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            try { 
                var existingUser = await _employeeRepo.FindByEmail(employee.Email);
            
                var validationResult = await _employeeValidator.ValidateAsync(employee);
             
                if(existingUser != null)
                {
                    throw new AlreadyExistsException("Employee with same email exists");
                }
                if (!validationResult.IsValid)
                    {
                    var errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                    throw new CustomeValidationException(errors, timeStamp);
                    }
                
                await _employeeRepo.AddEmployee(employee);
                return Ok("Record Created Successfully");
                }
            catch (CustomeValidationException customEx)
                {
                return BadRequest(new
                {
                    timeStamp = customEx.TimeStamp,
                    message = "Validation failed",
                    errors = customEx.Errors
                });
                }
            catch (Exception ex)
            {
                if (ex.Message.Contains("email address already exists"))
                    return BadRequest(new { timestamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region Modify Employee

        /// <summary>
        /// Modifies the details of an existing employee based on the provided employee ID. 
        /// This endpoint validates the updated employee data and is accessible only to users with the Admin role.
        /// </summary>
        /// <param name="employeeId">The ID of the employee to be modified.</param>
        /// <param name="employee">The updated employee data.</param>
        /// <returns>Returns a success message if the operation is successful, otherwise a Bad Request response with the validation or error details.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("Modify")]
        public async Task<IActionResult> ModifyEmployee(int employeeId, EmployeeDTO employee)
        {
            var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

            try
            {
                // Validate the incoming employee data
                var validationResult = await _employeeValidator.ValidateAsync(employee);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(error => error.ErrorMessage).ToList();
                    throw new CustomeValidationException(errors, timeStamp);
                }

                // Perform the modification
                await _employeeRepo.ModifyEmployee(employeeId, employee);
                return Ok("Record Modified Successfully");
            }
            catch (CustomeValidationException customEx)
            {
                // Handle custom validation exceptions
                return BadRequest(new
                {
                    timeStamp = customEx.TimeStamp,
                    message = "Validation failed",
                    errors = customEx.Errors 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    timeStamp = DateOnly.FromDateTime(DateTime.Now),
                    message = ex.Message
                });
            }
        }


        #endregion

        #region Assign Job

        /// <summary>
        /// Assigns a new job to an employee by replacing their current job. 
        /// This endpoint is accessible to users with the roles Admin or HR Team.
        /// </summary>
        /// <param name="currentJobId">The current job ID of the employee.</param>
        /// <param name="newJobId">The new job ID to be assigned to the employee.</param>
        /// <returns>Returns a success message if the operation is successful, otherwise a Bad Request response with the error details.</returns>
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

        #region Assign Manager

        /// <summary>
        /// Assigns a manager to an employee based on their IDs. 
        /// This endpoint is accessible to users with the roles Admin or HR Team.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="managerId">The ID of the manager to be assigned.</param>
        /// <returns>Returns a success message if the operation is successful, otherwise a Bad Request response with the error details.</returns>
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

        #region Assign Department

        /// <summary>
        /// Assigns an employee to a specific department based on their IDs. 
        /// This endpoint is accessible to users with the roles Admin, HR Team, or Employee.
        /// </summary>
        /// <param name="employeeId">The ID of the employee.</param>
        /// <param name="departmentId">The ID of the department to be assigned.</param>
        /// <returns>Returns a success message if the operation is successful, otherwise a Bad Request response with the error details.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpPut("Assign Department")]
        public async Task<IActionResult> AssignDep(decimal employeeId, decimal departmentId)
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

        #region Update Commission for Sales Department

        /// <summary>
        /// Updates the commission percentage for employees in the sales department. 
        /// This endpoint is accessible to users with the roles Admin or HR Team.
        /// </summary>
        /// <param name="departmentId">The ID of the sales department.</param>
        /// <param name="commissionPercentage">The new commission percentage to be applied.</param>
        /// <returns>Returns a success message if the operation is successful, otherwise a Bad Request response with the error details.</returns>
        [Authorize(Roles = "Admin,HR Team")]
        [HttpPut("update Commission for sales department")]
        public async Task<IActionResult> UpdateCommissionForDepartment(decimal departmentId, [FromQuery] decimal commissionPercentage)
        {
            try
            {
                await _employeeRepo.UpdateCommissionForDepartment(departmentId, commissionPercentage);
                return Ok("Record Modified Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }

        #endregion

        #region Find By First Name

        /// <summary>
        /// Searches for an employee by their first name. 
        /// This endpoint is accessible to users with the roles Admin, HR Team, or Employee.
        /// </summary>
        /// <param name="firstName">The first name of the employee to search for.</param>
        /// <returns>Returns the employee details if found, otherwise a Bad Request response with the error details.</returns>
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpGet("find by fisrt name")]
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

        #region Find By Email

        /// <summary>
        /// Searches for an employee by their email address. 
        /// This endpoint is accessible to users with the roles Admin, HR Team, or Employee.
        /// </summary>
        /// <param name="email">The email address of the employee to search for.</param>
        /// <returns>Returns the employee details if found, otherwise a Bad Request response with the error details.</returns>
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

        #region Find Employee By Phone Number

        /// <summary>
        /// Retrieves employee details based on the provided phone number. 
        /// This endpoint is accessible to users with the roles Admin, HR Team, or Employee.
        /// </summary>
        /// <param name="phone">The phone number of the employee to search for.</param>
        /// <returns>Returns the details of the employee if found, otherwise a Bad Request response.</returns>
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

        #region Find Employees With No Commission

        /// <summary>
        /// Fetches a list of all employees who do not receive any commission. 
        /// This endpoint is accessible to users with the roles Admin, HR Team, or Employee.
        /// </summary>
        /// <returns>Returns a list of employees without commissions or a Bad Request response if an error occurs.</returns>
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

        #region Total Commission Issued To Department

        /// <summary>
        /// Calculates the total commission amount issued to a specific department. 
        /// This endpoint is accessible to users with the roles Admin, HR Team, or Employee.
        /// </summary>
        /// <param name="departmentId">The ID of the department.</param>
        /// <returns>Returns the total commission issued to the department or a Bad Request response if an error occurs.</returns>
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

        #region List Employees By Department

        /// <summary>
        /// Retrieves a list of all employees within a specified department. 
        /// This endpoint is accessible to users with the roles Admin, HR Team, or Employee.
        /// </summary>
        /// <param name="departmentId">The ID of the department.</param>
        /// <returns>Returns a list of employees in the department or a Bad Request response if an error occurs.</returns>
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

        #region List Manager Details

        /// <summary>
        /// Retrieves details of all managers within the organization. 
        /// This endpoint is accessible to users with the roles Admin, HR Team, or Employee.
        /// </summary>
        /// <returns>Returns a list of managers or a Bad Request response if an error occurs.</returns>
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

        #region Employee Count By Location

        /// <summary>
        /// Counts all employees grouped by their respective office locations. 
        /// This endpoint is accessible to users with the roles Admin, HR Team, or Employee.
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

        #region Find Max Salary of Job By Employee ID

        /// <summary>
        /// Finds the maximum salary of the job associated with a specific employee ID. 
        /// This endpoint is accessible to users with the roles Admin, HR Team, or Employee.
        /// </summary>
        /// <param name="empid">The employee ID.</param>
        /// <returns>Returns the job title and maximum salary or a Bad Request response if an error occurs.</returns>
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

        #region Update Employee Email

        /// <summary>
        /// Updates the email address of a specific employee. 
        /// This endpoint is accessible to users with the roles Admin or HR Team.
        /// </summary>
        /// <param name="email">The new email address.</param>
        /// <param name="employeeDto">The employee details for the update.</param>
        /// <returns>Returns a success message or a Bad Request response if an error occurs.</returns>
        [Authorize(Roles = "Admin,HR Team")]
        [HttpPut("Update Email")]
        public async Task<IActionResult> UpdateEmployeeEmail(string currentemail, string newemail)
        {
            try
            {
<<<<<<< HEAD
=======
                var existingName = await _employeeRepo.FindByEmail(newemail);
                if(existingName != null)
                {
                    throw new AlreadyExistsException("New email already exists");
                }
>>>>>>> 496809e0f02fbd9324e188cef63b534505287cac
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

