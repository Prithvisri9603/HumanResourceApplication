﻿using Microsoft.EntityFrameworkCore;
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
        /*
        public string Authenticate(string username, string password)
        {
            var user = _context.Users.Include(u => u.Role).FirstOrDefault(u => u.Username == username && u.Passwordhash == password);

            if (user == null) return null;

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
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role.Name)
    }),
                Expires = DateTime.UtcNow.AddDays(7),
                Audience = audience,
                Issuer = issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }*/
 
}

}

