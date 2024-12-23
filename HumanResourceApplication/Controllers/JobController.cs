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

namespace HumanResourceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;
        private readonly JobsDTOValidator _validator;

        public JobController(IJobRepository jobRepository, JobsDTOValidator validator)
        {
            _jobRepository = jobRepository;
            _validator = validator;

        }

        #region Get Job methods
        [HttpGet]
        public async Task<IActionResult> GetJobs()
        {
            try
            {
                List<JobDTO> jobs = await _jobRepository.GetAllJobs();
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return BadRequest("Bad request bro");
            }
        }

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
        [HttpPost]
        public async Task<IActionResult> AddJob(JobDTO jobDTO)
        {
            try
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
            }
        }
        #endregion

        #region Update Job methods
        //UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateJob(string id, JobDTO jobDTO)
        {
            try
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
