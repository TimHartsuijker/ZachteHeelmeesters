using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowFrontend")]
    public class LoginController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AppDbContext context, ILogger<LoginController> logger)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _logger = logger;
        }
        

        [HttpGet]
        public IActionResult TestEndpoint()
        {
            _logger.LogInformation("Test endpoint called");
            return Ok(new { message = "API werkt!" });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation($"Login attempt for email: {request?.Email}");

            if (request == null ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Wachtwoord))
            {
                _logger.LogWarning("Login attempt with missing fields");
                return BadRequest(new { message = "Gegevens moeten ingevuld zijn" });
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                _logger.LogWarning($"Login attempt failed: user not found for email {request.Email}");
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
                _logger.LogWarning($"Login attempt failed: incorrect password for {request.Email}");
                return Unauthorized(new { message = "Inloggegevens zijn incorrect" });
            }

            _logger.LogInformation($"Login successful for {request.Email}");
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

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Doctor)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound(new { message = "Gebruiker niet gevonden" });
                }

                // Calculate age from DateOfBirth
                var today = DateTime.Today;
                var age = today.Year - user.DateOfBirth.Year;
                if (user.DateOfBirth.Date > today.AddYears(-age)) age--;

                return Ok(new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    fullName = $"{user.FirstName} {user.LastName}",
                    email = user.Email,
                    dateOfBirth = user.DateOfBirth,
                    age = age,
                    gender = user.Gender,
                    phoneNumber = user.PhoneNumber,
                    address = new
                    {
                        street = user.StreetName,
                        houseNumber = user.HouseNumber,
                        postalCode = user.PostalCode
                    },
                    doctor = user.Doctor != null ? new
                    {
                        id = user.Doctor.Id,
                        firstName = user.Doctor.FirstName,
                        lastName = user.Doctor.LastName,
                        fullName = $"{user.Doctor.FirstName} {user.Doctor.LastName}",
                        practiceName = user.Doctor.PracticeName
                    } : null,
                    role = user.Role.RoleName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Er is een fout opgetreden bij het ophalen van gebruikersgegevens" });
            }
        }

    }
}



 

        
