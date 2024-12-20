using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
using HumanResourceApplication.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HumanResourceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobHistoryController : ControllerBase
    {


        private readonly IJobHistoryRepository _repository;
        private readonly JobHistoryDTOValidator _validator;



        public JobHistoryController(IJobHistoryRepository repository, JobHistoryDTOValidator validator)
        {
            _repository = repository;
            _validator = validator;
        }
        /// <summary>
        /// Implemented required endpoints
        /// </summary>
        /// <returns></returns>


        //Get all job history available of all employees
        [HttpGet]
        public async Task<IActionResult> GetAllJobHistory()
        {
            try
            {
                List<JobHistoryDTO> jobHisList = await _repository.GetAllJobHistory();
                return Ok(jobHisList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        //Gets employees total years of experience 
        [HttpGet("{id}")]
        public async Task<IActionResult> totalyearsofexperience(decimal id)
        {
            try
            {
                var result = await _repository.FindExperienceOfEmployees(id);
                if (result == null)
                {
                    return NotFound();
                }
                int years = result.Value.Days / 365;
                int months = (result.Value.Days % 365) / 30;
                int days = (result.Value.Days % 365) % 30;

            
                    return Ok(new
                    {
                        years = years,
                        month = months,
                        days = days
                    });
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //Fetch employees with less than one year of experience
        [HttpGet("lessthanoneyearexperience/{emp_id}")]
        public async Task<IActionResult> ListAllEmployeesWithLessThanOneYearExperience(int emp_id)
        {
            try
            {
                var totalExperience = await _repository.GetTotalExperienceByEmployeeIdAsync(emp_id);

                if (totalExperience == null)
                {
                    return NotFound(new { message = "No job history found for this employee." });
                }

                // Calculate years, months, and days from the total experience
                int years = totalExperience.Value.Days / 365;
                int months = (totalExperience.Value.Days % 365) / 30;
                int days = (totalExperience.Value.Days % 365) % 30;

                if (years < 1)
                {
                    return Ok(new
                    {
                        years = years,
                        month = months,
                        days = days
                    });
                }

                return Ok(new { message = "Employee has one year or more of experience." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    timeStamp = DateTime.Now.ToString("yyyy-MM-dd"),
                    message = $"An error occurred: {ex.Message}"
                });
            }
        }

       
        // Adds new startdate to existing employee

        [HttpPost("{empid}/{startDate}/{jobId}/{deptId}")]
        public async Task<IActionResult> AddJobHistory(decimal empid, DateOnly startDate, string jobId, decimal deptId)
        {
            // Create a JobHistoryRequest instance
            var request = new JobHistoryDTO
            {
                EmployeeId = empid,
                StartDate = startDate,
                JobId = jobId,
                DepartmentId = deptId
            };

            // Validate using FluentValidation
            var validator = new JobHistoryDTOValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            try
            {
                await _repository.AddJobHistory(empid, startDate, jobId, deptId);
                return Ok("Record Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //public async Task<IActionResult> AddJobHistory(decimal empid,DateOnly startDate,string jobId,decimal deptId)
        //{
        //    try
        //    {
        //        await _repository.AddJobHistory(empid, startDate, jobId, deptId);
        //        return Ok("Record Created Successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //}


        //Updates employeeId and startDate on JobHistory table
        [HttpPut("{empid}/{startDate}")]
        public async Task<IActionResult> UpdateJobHistory(decimal empid, DateOnly startDate, [FromQuery]DateOnly enddate)
        {
            try
            {
                await _repository.UpdateJobHistory(empid, startDate, enddate);
                return Ok("Record Modified Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
