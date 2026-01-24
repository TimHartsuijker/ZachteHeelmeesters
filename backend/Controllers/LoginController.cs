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

            // 🔒 Admin accounts moeten via admin login pagina
            if (user.Role.RoleName == "Admin")
            {
                _logger.LogWarning($"Admin account {request.Email} tried to login via normal login");
                return Unauthorized(new { message = "Gebruik de admin login pagina voor deze account" });
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Wachtwoord
            );

            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning($"Incorrect password for {request.Email}");
                return Unauthorized(new { message = "Inloggegevens zijn incorrect" });
            }

            _logger.LogInformation($"Login successful for {request.Email} (Role: {user.Role.RoleName})");

            return Ok(new
            {
                message = "Login succesvol",
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
            _logger.LogInformation($"Admin login attempt for email: {request?.Email}");

            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Wachtwoord))
            {
                _logger.LogWarning("Admin login attempt with missing fields");
                return BadRequest(new { message = "Gegevens moeten ingevuld zijn" });
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            // ✅ ALLEEN Admin accounts via deze login
            if (user == null || user.Role.RoleName != "Admin")
            {
                _logger.LogWarning($"Unauthorized admin login attempt for {request.Email}");
                return Unauthorized(new { message = "Inloggegevens zijn incorrect of geen admin account" });
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Wachtwoord
            );

            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning($"Admin login incorrect password for {request.Email}");
                return Unauthorized(new { message = "Inloggegevens zijn incorrect" });
            }

            _logger.LogInformation($"Admin login successful for {request.Email}");

            return Ok(new
            {
                message = "Admin login succesvol",
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
                _logger.LogError(ex, "Error getting user details for userId: {UserId}", userId);
                return StatusCode(500, new { message = "Er is een fout opgetreden bij het ophalen van gebruikersgegevens" });
            }
        }
    }
}