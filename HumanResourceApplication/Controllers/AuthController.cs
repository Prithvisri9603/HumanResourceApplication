using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HumanResourceApplication.DTO;
using HumanResourceApplication.Services;

namespace HumanResourceApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authservice;

        public AuthController(IAuthServices authRepository)
        {
            _authservice = authRepository;
        }
/*

        [HttpPost("Login")]
        public IActionResult Authenticate([FromBody] LoginDTO loginDTO)
        {
            string token = _authservice.Authenticate(loginDTO.Username, loginDTO.Password);
            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(token);
      }*/

    }
}

