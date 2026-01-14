using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public LoginController(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }
        

        [HttpGet]
        public IActionResult TestEndpoint()
        {
            return Ok(new { message = "API werkt!" });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Wachtwoord))
            {
                return BadRequest(new { message = "Gegevens moeten ingevuld zijn" });
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized(new { message = "Inloggegevens zijn incorrect" });
            }

            // 🔒 ADMIN MAG HIER NIET INLOGGEN
            if (user.Role.RoleName == "Admin")
            {
                return Unauthorized(new { message = "Gebruik de admin login pagina" });
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Wachtwoord
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Inloggegevens zijn incorrect" });
            }

            return Ok(new
            {
                message = "Login ok",
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    role = user.Role.RoleName
                }
            });
        }
        [HttpPost("admin")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Wachtwoord))
            {
                return BadRequest(new { message = "Gegevens moeten ingevuld zijn" });
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || user.Role.RoleName != "Admin")
            {
                return Unauthorized(new { message = "Inloggegevens zijn incorrect" });
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Wachtwoord
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new { message = "Inloggegevens zijn incorrect" });
            }

            return Ok(new
            {
                message = "Login ok",
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    role = user.Role.RoleName
                }
            });
        }

    }
}



 

        
