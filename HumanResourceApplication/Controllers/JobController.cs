using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace HumanResourceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly IJobRepository _jobRepository;

        public JobController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;

        }

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


        //Add Job 
        [HttpPost]
        public async Task<IActionResult> AddJob(JobDTO jobDTO)
        {
            try
            {
                if (jobDTO == null)
                {
                    return BadRequest();
                }
                else if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                await _jobRepository.AddJob(jobDTO);
                return Ok("Record Created Successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Update Job table
        [HttpPut("{id}")]
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

        [HttpPut("minsalary/maxsalary{id}")]
        public async Task<IActionResult> UpdateJobMinAndMaxSalary(string id,decimal newMin,decimal newMax)
        {
            try
            {
                await _jobRepository.UpdateJobMinAndMaxSalary(id,newMin, newMax);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}
