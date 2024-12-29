using FluentValidation;
using HumanResourceApplication.DTO;
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
    public class JobHistoryController : ControllerBase
    {


        private readonly IJobHistoryRepository _repository;
        private readonly IValidator<JobHistoryDTO> _validator;

        public JobHistoryController(IJobHistoryRepository repository, IValidator<JobHistoryDTO> validator)
        {
            _repository = repository;
            _validator = validator;
        }


        /// <summary>
        /// Implemented required endpoints
        /// </summary>
        /// <returns></returns>
        //Get all job history available of all employees

        #region Get All Job History
        [Authorize(Roles = "Admin, HR Team, Employee")]
        [HttpGet]
        public async Task<IActionResult> GetAllJobHistory()
        {
            try
            {
                List<JobHistoryDTO> jobHisList = await _repository.GetAllJobHistory();
                if (jobHisList == null || jobHisList.Count == 0)
                {
                    return NotFound(new { message = "No job history found." });
                }
                return Ok(jobHisList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Total years of experience
        //Gets employees total years of experience 
        [Authorize(Roles = "Admin, HR Team")]
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
        #endregion

        #region Employees with less than a year of experience
        //Fetch employees with less than one year of experience
        [Authorize(Roles = "Admin, HR Team")]
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
        #endregion


        #region Add JobHistory
        // Adds new startdate to existing employee
        [Authorize(Roles = "Admin")]
        [HttpPost("{empid}/{startDate}/{jobId}/{deptId}")]
        /*
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
        }*/

        public async Task<IActionResult> AddJobHistory(decimal empid, DateOnly startDate, string jobId, decimal deptId)
        {
            try
            {
                var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

                // Create a JobHistoryRequest instance
                var request = new JobHistoryDTO
                {
                    EmployeeId = empid,
                    StartDate = startDate,
                    JobId = jobId,
                    DepartmentId = deptId
                };

                // Check if the job history already exists
                var existingJobHistory = await _repository.GetJobHistoryByEmployeeIdAndJob(empid, jobId);
                if (existingJobHistory != null)
                {
                    throw new AlreadyExistsException($"Job history for employee {empid} and job {jobId} already exists.");
                }

                // Validate the JobHistoryDTO using FluentValidation
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    throw new CustomeValidationException(errors, timeStamp);
                }

                // Add JobHistory to the repository
                await _repository.AddJobHistory(empid, startDate, jobId, deptId);

                // Return successful response
                return Ok(new
                {
                    Message = "Record Created Successfully"
                });
            }
            catch (Exception ex)
            {
                // Return error response with timestamp
                return BadRequest(new
                {
                    TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Message = ex.Message
                });
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
        #endregion

        #region Update History
        //Updates employeeId and startDate on JobHistory table
        [Authorize(Roles = "Admin, HR Team")]
        [HttpPut("{empid}/{startDate}")]
        /*
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
        }*/

        public async Task<IActionResult> UpdateJobHistory(decimal empid, DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            try
            {
                var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

                // Create the JobHistoryDTO for validation
                var request = new JobHistoryDTO
                {
                    EmployeeId = empid,
                    StartDate = startDate,
                    EndDate = endDate
                };

                // Validate the JobHistoryDTO using FluentValidation
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    throw new CustomeValidationException(validationResult.Errors.Select(e => e.ErrorMessage).ToList(), timeStamp);
                }

                // Check if the JobHistory exists
                var existingJobHistory = await _repository.GetJobHistoryByEmployeeIdAndJob(request.EmployeeId, request.JobId);  // Assuming you have the jobId in your DTO or pass it
                if (existingJobHistory == null)
                {
                    throw new AlreadyExistsException("Job history not found.");
                }

                // Update the JobHistory in the repository
                await _repository.UpdateJobHistory(empid, startDate, endDate);

                return Ok(new
                {
                    Message = "Record Modified Successfully"
                });
            }
            catch (Exception ex)
            {
                // Return error response with timestamp
                return BadRequest(new
                {
                    TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Message = ex.Message
                });
            }
        }

        #endregion
    }
}
