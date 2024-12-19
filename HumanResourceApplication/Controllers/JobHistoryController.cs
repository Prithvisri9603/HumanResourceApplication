using HumanResourceApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HumanResourceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobHistoryController : ControllerBase
    {
        private readonly IJobHistoryRepository _repository;
        public JobHistoryController(IJobHistoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> totalyearsofexperience(decimal id)
        {
            try
            {
                var result = await _repository.FindExperienceOfEmployees(id);
                if (result == (0, 0, 0))
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        

    }
}
