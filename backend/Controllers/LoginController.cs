using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendLogin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public LoginController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Wachtwoord))
            {
                return BadRequest(new { message = "Gegevens moeten ingevuld zijn" });
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized(new { message = "Inloggegevens zijn incorrect" });

            var result = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Wachtwoord);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized(new { message = "Inloggegevens zijn incorrect" });

            // Simpel: stuur user info terug
            return Ok(new
            {
                message = "Login ok",
                user = new
                {
                    id = user.Id,
                    email = user.Email,
                    role = user.Role.RoleName
                }
            });
        }
    }
}

