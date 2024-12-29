using Azure.Core;
using FluentValidation;
using HumanResourceApplication.Validators;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using HumanResourceApplication.Utility;


namespace HumanResourceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;
        private readonly IValidator<JobDTO> _validator;

        public JobController(IJobRepository jobRepository, IValidator<JobDTO> validator)
        {
            _jobRepository = jobRepository;
            _validator = validator;

        }

        #region Get methods
        [Authorize(Roles = "Admin,HR Team,Employee")]
        [HttpGet]
        public async Task<IActionResult> GetJobs()
        {
            try
            {
                List<JobDTO> jobs = await _jobRepository.GetAllJobs();
                if (jobs.Count == 0 || jobs == null)
                {
                    return NotFound(new { message = "No job found" });
                }
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad request bro");
            }
        }
        
        [Authorize(Roles = "Admin,HR Team")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetJobById(string id)
        {
            try
            {
                JobDTO jobDTO = await _jobRepository.GetJobById(id);
                if (jobDTO == null)
                {
                    return NotFound($"JobID {id} is not found");

                }
                return Ok(jobDTO);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        
        #region Add job
        //Add Job 

        //public async Task<IActionResult> AddJob(JobDTO jobDTO)
        //{
        //    try
        //    {
        //        if (jobDTO == null)
        //        {
        //            return BadRequest();
        //        }
        //        else if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        await _jobRepository.AddJob(jobDTO);
        //        return Ok("Record Created Successfully");
        //    }

        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddJob(JobDTO jobDTO)
        {
            /*try
            {
               
                // Explicitly validate the JobDTO
                var validationResult = await _validator.ValidateAsync(jobDTO);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                await _jobRepository.AddJob(jobDTO);
                return Ok("Record Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }*/

            try
            {
                // Capture the current timestamp in UTC ISO 8601 format
                var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

                // Check if the job already exists (based on JobId or a unique field)
                var existingJob = await _jobRepository.GetJobById(jobDTO.JobId);
                if (existingJob != null)
                {
                    throw new AlreadyExistsException($"Job with ID '{jobDTO.JobId}' already exists.");
                }

                // Validate the job DTO using the validator
                var validationResult = await _validator.ValidateAsync(jobDTO);
                if (!validationResult.IsValid)
                {
                    // If validation fails, collect the errors and throw a custom exception
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    throw new CustomeValidationException(errors, timeStamp);
                }

                // Add the new job to the repository
                await _jobRepository.AddJob(jobDTO);

                // Return success response with timestamp
                return Ok(new
                {
                    //TimeStamp = timeStamp,
                    Message = "Job record created successfully"
                });
            }
            
            catch (Exception ex)
            {
                // Return BadRequest for any other exceptions
                return BadRequest(new
                {
                    TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    //Message = "An error occurred while adding the job.",
                    Details = ex.Message
                });
            }
        }

        #endregion

        #region Update Job methods
        //UPDATE
        [Authorize(Roles = "Admin,HR Team")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(string id, JobDTO jobDTO)
        {
            /*try
            {
                var validationResult = await _validator.ValidateAsync(jobDTO);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }
                await _jobRepository.UpdateJob(id, jobDTO);
                return Ok("Record Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }*/

            
            try
            {
                // Capture the current timestamp in UTC ISO 8601 format
                var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");

                // Validate the job DTO using the validator
                var validationResult = await _validator.ValidateAsync(jobDTO);
                if (!validationResult.IsValid)
                {
                    // Return a BadRequest response with validation errors if validation fails
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(new
                    {
                        TimeStamp = timeStamp,
                        Message = "Validation failed",
                        Errors = errors
                    });
                }

                // Check if the job exists in the repository (optional, depending on your logic)
                var existingJob = await _jobRepository.GetJobById(id);
                if (existingJob == null)
                {
                    throw new AlreadyExistsException($"Job with ID '{id}' not found.");
                }

                // Update the job if it exists
                await _jobRepository.UpdateJob(id, jobDTO);

                // Return a successful response
                return Ok(new
                {
                    //TimeStamp = timeStamp,
                    Message = "Job record updated successfully"
                });
            }
            
            catch (Exception ex)
            {
                // Return a general error message in case of unexpected exceptions
                return BadRequest(new
                {
                    TimeStamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    //Message = "An error occurred while updating the job.",
                    Details = ex.Message
                });
            }
        }

        

        /* [HttpPut("{id}")]
         public async Task<IActionResult> UpdateJob(string id,JobDTO jobDTO)
         {
             try
             {
                 if (jobDTO == null)
                 {
                     return BadRequest($"Job {id} was not found");
                 }
                 await _jobRepository.UpdateJob(id, jobDTO);
                 return Ok("Record Modified Successfully");

             }
             catch (Exception ex)
             {
                 return BadRequest(ex.Message);
             }
         }
        */



        [Authorize(Roles = "Admin,HR Team")]
        [HttpPut("minsalary/maxsalary{id}")]
        public async Task<IActionResult> UpdateJobMinAndMaxSalary(string id,decimal newMin,decimal newMax)
        {
            try
            {
                await _jobRepository.UpdateJobMinAndMaxSalary(id,newMin, newMax);
                return Ok("Success");
                //return CreatedAtAction("GetJobById", new { id = });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        #endregion

    }
}
