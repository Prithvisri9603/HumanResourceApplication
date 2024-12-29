using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HumanResourceApplication.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HumanResourceApplication.Services;

namespace HumanResourceApplication.Services
{
    public class AuthServices:IAuthServices
    {
        private readonly HrContext _context;
        private readonly IConfiguration _configuration;

        public AuthServices(HrContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        
        #region Authenticate

        /// <summary>
        /// Authenticates a user by validating their username and password, and generates a JWT token upon successful authentication.
        /// </summary>
        /// <param name="username">The username of the user attempting to authenticate.</param>
        /// <param name="password">The password of the user attempting to authenticate.</param>
        /// <returns>
        /// A JWT token string if authentication is successful; otherwise, returns null if the user credentials are invalid.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the configuration is not initialized or when the JWT settings (Key, Audience, Issuer) are missing or invalid.
        /// </exception>

        public string Authenticate(string username, string password)
        {
            //var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == username && u.Passwordhash == password);


            //         var ruser = _context.Users
            //.FromSqlInterpolated($@"
            // SELECT * 
            //FROM Users
            //WHERE Username = {username} 
            //AND Passwordhash= CONVERT(VARCHAR(255), HASHBYTES('SHA2_256', {password}), 2)")
            //.Include(u => u.Role)
            //.FirstOrDefault();

            var ruser = _context.Users
    .FromSqlInterpolated($@"
        EXEC GetUserByUsernameAndPassword @Username = {username}, @Password = {password}")
    .AsEnumerable()
    .FirstOrDefault();

            if(ruser!=null)
            {
                ruser.Role = _context.Roles.FirstOrDefault(r => r.Id == ruser.RoleId);
            }


            if (ruser == null) return null;

            if (_configuration == null)
            {
                throw new InvalidOperationException("Configuration is not initialized");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }

            var audience = _configuration["Jwt:Audience"];
            var issuer = _configuration["Jwt:Issuer"];
            if (string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(issuer))
            {
                throw new InvalidOperationException("JWT Audience or Issuer is not configured.");
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
        new Claim(ClaimTypes.Name, ruser.Username),
        new Claim(ClaimTypes.Role, ruser.Role.Name)
    }),
                Expires = DateTime.UtcNow.AddDays(7),
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        #endregion
        
    }

}

