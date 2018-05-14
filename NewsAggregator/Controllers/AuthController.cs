using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NewsAggregator.Models;

namespace NewsAggregator.Controllers
{
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config, ILogger<AuthController> logger)
        {
            _config = config;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody] Login login)
        {
            IActionResult response = Unauthorized();
            var user = Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken();
                response = Ok(new {token = tokenString});
                _logger.LogInformation("Successfull authorization");
            }

            return response;
        }

        private string BuildToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            _logger.LogInformation("Token has been created");

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User Authenticate(Login login)
        {
            User user = null;

            if (login.Username == "username" && login.Password == "password")
            {
                user = new User
                {
                    FirstName = "User Name",
                    LastName = "User Surname",
                    Username = login.Username,
                    Password = login.Password
                };
                _logger.LogInformation("User has been authenticated");
            }
            else _logger.LogWarning("Wrong credentials whlie authenticating");

            return user;
        }
    }
}