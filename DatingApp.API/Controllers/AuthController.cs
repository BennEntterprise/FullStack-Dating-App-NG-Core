using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using datingapp.api.Dtos;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController] // Forces attribute routing
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
        {

            // convert username to lowercase
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exists");

            var userToCreate = new User
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            try
            {
                throw new Exception("Computer says no!");

                //Make sure we have a user
                var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);
                if (userFromRepo == null)
                    return Unauthorized(); //Non-discriminat rejections. 

                // Set up claims array.
                var claims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()), //Claim(TypeOfClaimImAsserting, ValueOfAssertion)
                new Claim(ClaimTypes.Name, userFromRepo.Username)  // **Checkout other "ClaimTypes"! It's basically ~free~ validation code!
            };

                // Server side signing of a token that will be given to the client
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1),
                    SigningCredentials = creds
                };

                //Make the JWT based on the descriptor. 
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Write the token into the response!
                return Ok(new
                {
                    token = tokenHandler.WriteToken(token)
                });
            }
            catch
            {
                return StatusCode(500, "Computer Really Says No");
            }
        }
    }
}

