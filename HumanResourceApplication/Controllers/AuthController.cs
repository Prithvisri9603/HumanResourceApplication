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
        
        #region Login

        /// <summary>
        /// Authenticates a user using their username and password.
        /// </summary>
        /// <param name="loginDTO">The login credentials containing username and password.</param>
        /// <returns>
        /// Returns an HTTP 200 OK response with the generated token if authentication is successful.
        /// Returns an HTTP 401 Unauthorized response if authentication fails.
        /// </returns>

        [HttpPost("Login")]
        public IActionResult Authenticate([FromBody] LoginDTO loginDTO)
        {
            string token = _authservice.Authenticate(loginDTO.Username, loginDTO.Password);
            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(token);
        }
        #endregion
        
    }
}

