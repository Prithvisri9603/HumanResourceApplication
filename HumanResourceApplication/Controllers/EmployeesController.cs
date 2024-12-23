using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;
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
        private readonly EmployeeValidator _employeeValidator;
        public EmployeesController(IEmployeeRepo employeeRepo,EmployeeValidator employeevalidator)
        {
            _employeeRepo = employeeRepo;
            _employeeValidator = employeevalidator;
        }
       [Authorize(Roles ="Admin")]
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
            }
            catch (Exception ex)
            {
                if(ex.Message.Contains("email address already exists"))
                    return BadRequest(new {timestamp=DateOnly.FromDateTime(DateTime.Now), message = ex.Message});
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }
        [Authorize(Roles ="Admin")]
        [HttpPut("Modify")]
        public async Task<IActionResult> ModifyEmployee(int employeeId, EmployeeDTO employee)
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
                await _employeeRepo.ModifyEmployee(employeeId, employee);
                return Ok("Record Modified Successfully");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("email address already exists"))
                    return BadRequest(new {timestamp= DateOnly.FromDateTime(DateTime.Now),message = ex.Message});
                return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }
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
                return BadRequest(new{timeStamp = DateOnly.FromDateTime(DateTime.Now),message = ex.Message});
            }
        }

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
        [Authorize(Roles ="Admin,HR Team,Employee")]
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
        [Authorize(Roles = "Admin,HR Team")]
        [HttpPut("update Commission for sales department")]
        public async Task<IActionResult> UpdateCommissionForDepartment(decimal departmentId, [FromQuery] decimal commissionPercentage)
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
        [Authorize(Roles = "Admin,HR Team")]
        [HttpPut("Update Email")]
        public async Task<IActionResult> UpdateEmployeeEmail(string email, EmployeeDTO employeeDto)
        {
            try
            {
                await _employeeRepo.UpdateEmployeeEmail(email, employeeDto);
                return Ok("Record Modified Successfully");
            }
            catch (Exception ex)
            {
            return BadRequest(new { timeStamp = DateOnly.FromDateTime(DateTime.Now), message = ex.Message });
            }
        }
    }
}

