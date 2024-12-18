using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
                return BadRequest(ex.Message);
            }
        }
    }
}
