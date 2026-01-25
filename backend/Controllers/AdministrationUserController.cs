using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Models;
using backend.DTOs;
using Microsoft.AspNetCore.Identity;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminUserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly ILogger<AdminUserController> _logger;

        public AdminUserController(AppDbContext context, ILogger<AdminUserController> logger)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _logger = logger;
        }

        // POST: api/AdminUser/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestDto request)
        {
            try
            {
                _logger.LogInformation($"Create user attempt for email: {request?.Email}");

                // Validatie
                if (request == null)
                    return BadRequest(new { message = "Gegevens zijn vereist" });

                if (string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.FirstName) ||
                    string.IsNullOrWhiteSpace(request.LastName) ||
                    request.RoleId <= 0)
                {
                    return BadRequest(new { message = "Alle verplichte velden moeten worden ingevuld" });
                }

                // Controleer of email al bestaat
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (existingUser != null)
                    return BadRequest(new { message = "Een gebruiker met dit e-mailadres bestaat al" });

                // Vind de geselecteerde rol
                var role = await _context.Roles.FindAsync(request.RoleId);
                if (role == null)
                    return BadRequest(new { message = "Ongeldige rol geselecteerd" });

                // 🔧 Genereer wachtwoord als die niet is meegegeven
                var password = string.IsNullOrWhiteSpace(request.Password)
                    ? GenerateTemporaryPassword()
                    : request.Password;

                // Maak nieuwe gebruiker aan met ALLE verplichte velden
                var newUser = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    StreetName = request.StreetName ?? "Niet opgegeven",
                    HouseNumber = request.HouseNumber ?? "0",
                    PostalCode = request.PostalCode ?? "0000AA",
                    CitizenServiceNumber = request.CitizenServiceNumber ?? GenerateRandomBSN(),
                    DateOfBirth = request.DateOfBirth ?? DateTime.UtcNow.AddYears(-30),
                    Gender = request.Gender ?? "Zeg ik liever niet",
                    PhoneNumber = request.PhoneNumber,
                    RoleId = request.RoleId,
                    CreatedAt = DateTime.UtcNow,
                    // PracticeName wordt niet meer gebruikt
                    PracticeName = null
                };

                // Hash het wachtwoord
                newUser.PasswordHash = _passwordHasher.HashPassword(newUser, password);

                // Opslaan
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User created successfully: {newUser.Email}, Role: {role.RoleName}");

                return Ok(new
                {
                    message = "Gebruiker succesvol aangemaakt",
                    user = new
                    {
                        id = newUser.Id,
                        email = newUser.Email,
                        firstName = newUser.FirstName,
                        lastName = newUser.LastName,
                        role = role.RoleName
                    },
                    temporaryPassword = password // Ter info voor admin
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij aanmaken gebruiker");
                return StatusCode(500, new
                {
                    message = "Er is een fout opgetreden bij het aanmaken van de gebruiker",
                    error = ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        private string GenerateTemporaryPassword()
        {
            // Genereer een veilig wachtwoord
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GenerateRandomBSN()
        {
            var random = new Random();
            return random.Next(100000000, 999999999).ToString();
        }
    }
}