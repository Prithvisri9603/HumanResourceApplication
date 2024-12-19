using HumanResourceApplication.DTO;
using HumanResourceApplication.Models;
using HumanResourceApplication.Services;
using HumanResourceApplication.Validators;
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
        [HttpPost("Add new Employee ")]
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
                return BadRequest(new { timeStamp = DateTime.UtcNow, message = ex.Message });
            }
        }
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
                return BadRequest(new { timeStamp = DateTime.UtcNow, message = ex.Message });
            }
        }
        [HttpPut("Assign Job")]
        public async Task<IActionResult> AssignJob(string JobId, EmployeeDTO employee)
        {
            try
            {
                await _employeeRepo.AssignJob(JobId, employee);
                return Ok("Record Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Assign Manager")]
        public async Task<IActionResult> AssignMan(decimal ManagerId, EmployeeDTO employee)
        {
            try
            {
                await _employeeRepo.AssignMan(ManagerId, employee);
                return Ok("Record Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("Assign Department")]
        public async Task<IActionResult> AssignDep(decimal DepartmentId, EmployeeDTO employee)
        {
            try
            {
                await _employeeRepo.AssignDep(DepartmentId, employee);
                return Ok("Record Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
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
                return BadRequest(ex.Message);
            }
        }
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
                return BadRequest(ex.Message);
            }
        }

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
                    return BadRequest(new { timeStamp = DateTime.UtcNow, message = ex.Message });
                }
            }

            [HttpGet("findphone/{phone}")]
            public async Task<IActionResult> FindByPhoneNumber(string phone)
            {
                try
                {
                    var employee = await _employeeRepo.FindByPhoneNumber(phone);
                    return Ok(employee);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { timeStamp = DateTime.UtcNow, message = ex.Message });
                }
            }

            [HttpGet("findAllEmployeeWithNoCommission")]
            public async Task<IActionResult> FindAllEmployeeWithNoCommission()
            {
                try
                {
                    var employees = await _employeeRepo.FindAllEmployeeWithNoCommission();
                    return Ok(employees);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { timeStamp = DateTime.UtcNow, message = ex.Message });
                }
            }

            [HttpGet("findTotalCommissionIssuedToEmployee/{departmentId}")]
            public async Task<IActionResult> FindTotalCommissionIssuedToDepartment(decimal departmentId)
            {
                try
                {
                    var totalCommission = await _employeeRepo.FindTotalCommissionIssuedToDepartment(departmentId);
                    return Ok(new { departmentId, sum = totalCommission });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { timeStamp = DateTime.UtcNow, message = ex.Message });
                }
            }

            [HttpGet("listAllEmployees/{departmentId}")]
            public async Task<IActionResult> ListAllEmployeesByDepartment(decimal departmentId)
            {
                try
                {
                    var employees = await _employeeRepo.ListAllEmployeesByDepartment(departmentId);
                    return Ok(employees);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { timeStamp = DateTime.UtcNow, message = ex.Message });
                }
            }

            [HttpGet("listAllManagerDetails")]
            public async Task<IActionResult> ListAllManagerDetails()
            {
                try
                {
                    var managers = await _employeeRepo.ListAllManagerDetails();
                    return Ok(managers);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { timeStamp = DateTime.UtcNow, message = ex.Message });
                }
            }

            [HttpGet("locationwisecountofemployees")]
            public async Task<IActionResult> CountAllEmployeesGroupByLocation()
            {
                try
                {
                    var result = await _employeeRepo.CountAllEmployeesGroupByLocation();
                    return Ok(result);
                }
                catch (Exception ex)
                {
                    return BadRequest(new { timeStamp = DateTime.UtcNow, message = ex.Message });
                }
            }
        [HttpGet("{empid}/findmaxsalaryofjob")]
        public async Task<IActionResult> FindMaxSalaryOfJobByEmployeeId(decimal empid)
        {
            try
            {
                var result = await _employeeRepo.FindMaxSalaryOfJobByEmployeeId(empid);
                return Ok(new { JobTitle = result.JobDescription, MaxSalary = result.MaxSalary });
            }
            catch (Exception ex)
            {
                return BadRequest(new { timeStamp = DateTime.UtcNow, message = ex.Message });
            }
        }

        [HttpPut("{email}")]
            public async Task<IActionResult> UpdateEmployeeEmail(string email, EmployeeDTO employeeDto)
            {
                try
                {
                    await _employeeRepo.UpdateEmployeeEmail(email, employeeDto);
                    return Ok("Record Modified Successfully");
                }
                catch (Exception ex)
                {
                    return BadRequest(new { timeStamp = DateTime.UtcNow, message = ex.Message });
                }
            }
        }
    }

