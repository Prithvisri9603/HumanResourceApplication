using AutoMapper;
using FluentValidation;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Bson;

namespace HumanResourceApplication.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentRepository _departmentrepository;
        private readonly IValidator<DepartmentDTO> _departmentValidator;
        private readonly IConfiguration _configuration;

        public DepartmentController(IDepartmentRepository departmentrepository, IValidator<DepartmentDTO> departmentValidator, IConfiguration configuration)
        {
            _departmentrepository = departmentrepository;
            _departmentValidator = departmentValidator;
            _configuration = configuration;
        }
        #region AddDepartment
        [Authorize(Roles = "Admin")]
        [HttpPost("AddDepartment")]
        public async Task<IActionResult> AddDepartment(DepartmentDTO department)
        {
            /* var validationResult = _departmentValidator.Validate(department);
             if (!validationResult.IsValid)
             {
                 return BadRequest("Validation failed");
             }
             try
             {
                 if (department == null)
                 {
                     return BadRequest();
                 }

                 await _departmentrepository.AddDepartment(department);
                 return Ok("Record created successfully");
             }
             catch (Exception)
             {
                 return BadRequest(new { Message = "An error occurred." });
             }*/


            try
            {
                // Validate the department DTO
                var validationResult = _departmentValidator.Validate(department);
                if (!validationResult.IsValid)
                {
                    // Return BadRequest with the validation errors
                    return BadRequest(new
                    {
                        Message = "Validation failed",
                        Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                    });
                }
                // Check if the department object is null
                if (department == null)
                {
                    return BadRequest(new { Message = "Department data cannot be null." });
                }

                // Add the department to the repository
                await _departmentrepository.AddDepartment(department);

                // Return success message
                return Ok(new { Message = "Department created successfully" });
            }
            catch (Exception ex)
            {
                // Return BadRequest with a generic error message and exception details
                return BadRequest(new
                {
                    Message = "An error occurred while adding the department.",
                    Details = ex.Message
                });
            }
        }
        #endregion

        #region GetDepartment
        [Authorize(Roles = "Admin, HR Team, Employee")]
        [HttpGet("GetAllDepartment")]

        public async Task<IActionResult> GetDepartment()
        {
            try
            {
                List<DepartmentDTO> departmentlist = await _departmentrepository.GetDepartment();
                return Ok(departmentlist);
            }
            catch (Exception)
            {
                return BadRequest(new { Message = "An error occurred." });
            }
        }
        #endregion

        #region UpdateDepartment
        [Authorize(Roles = "Admin, HR Team")]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateDepartment(decimal departmentId, DepartmentDTO departmentdto)
        {
            /*try
            {
                var validationresult = _departmentValidator.Validate(departmentdto);
                if (!validationresult.IsValid)
                {
                    return BadRequest("Validation failed");
                }
                await _departmentrepository.UpdateDepartment(departmentId, departmentdto);
                return Ok("Record Modified successfully");
            }
            catch (Exception)
            {
                return BadRequest(new { Message = "An error occurred." });
            }*/
           
        
            try
            {
                // Validate the department DTO
                var validationresult = _departmentValidator.Validate(departmentdto);
                if (!validationresult.IsValid)
                {
                    // Return BadRequest with the validation errors
                    return BadRequest(new
                    {
                        Message = "Validation failed",
                        Errors = validationresult.Errors.Select(e => e.ErrorMessage).ToList()
                    });
                }

                // Check if the department DTO is null
                if (departmentdto == null)
                {
                    return BadRequest(new { Message = "Department data cannot be null." });
                }

                // Update the department in the repository
                await _departmentrepository.UpdateDepartment(departmentId, departmentdto);

                // Return success message
                return Ok(new { Message = "Department record updated successfully" });
            }
            catch (Exception ex)
            {
                // Return a BadRequest with the error details in case of any exception
                return BadRequest(new
                {
                    Message = "An error occu.",
                    Details = ex.Message
                });
            }
        }

        
        #endregion

        #region GetMaximumSalary
        [Authorize(Roles = "Admin, HR Team")]
        [HttpGet("findmaxsalary/{department_id}")]
         
        public async Task<IActionResult> GetMaximumSalary(decimal department_id)
        {
            try
            {
                var result = await _departmentrepository.GetMaximumSalary(department_id);
                if(result==null || !result.Any())
                {
                    return NotFound("No data found by this id");
                }
                return Ok(result);
            }
            catch(Exception)
            {
                return BadRequest(new { Message = "An error occurred." });
            }


        }
        #endregion

        #region GetMinSalary
        //min salary
        [Authorize(Roles = "Admin, HR Team")]
        [HttpGet("findminsalary/{department_id}")]

        public async Task<IActionResult> GetMinSalary(decimal department_id)
        {
            try
            {
                var result = await _departmentrepository.GetMinSalary(department_id);
                if (result == null || !result.Any())
                {
                    return NotFound("No data found by this id");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }


        }
        #endregion GetAllDeptDetailsForEmp

        #region GetAllDeptDetailsForEmp
        [Authorize(Roles = "Admin, HR Team")]
        [HttpGet("{Emp_id}")]

        public async Task<IActionResult> GetAllDeptDetailsForEmp(decimal Emp_id)
        {
            try
            {
                var result = await _departmentrepository.GetAllDeptDetailsForEmp(Emp_id);
                if (result == null || !result.Any())
                {
                    return NotFound(new { Message = "No departments found for the given employee ID." });
                }

                // Return the result as a JSON response
                return Ok(result);


            }
            catch (Exception)
            {
                return BadRequest(new { Message = "An error occurred."});
            }
        }


        #endregion

        #region DeleteDepartmentById
        [Authorize(Roles = "Admin")]
        [HttpDelete("{department_id}/Delete")]

        public async Task<IActionResult> DeleteDepartmentById(decimal department_id)
        {
            try
            {
                await _departmentrepository.DeleteDepartmentById(department_id);
                return Ok("Record deleted succesfully");
            }
            catch (Exception)
            {
                return BadRequest(new { Message = "An error occurred." });
            }
        }
        #endregion
    }
}
