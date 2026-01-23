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
                // AC6.10.3: Validatie van ingevulde gegevens
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

                // Valideer BSN indien aanwezig
                if (!string.IsNullOrWhiteSpace(request.CitizenServiceNumber))
                {
                    if (!System.Text.RegularExpressions.Regex.IsMatch(request.CitizenServiceNumber, @"^\d{9}$"))
                        return BadRequest(new { message = "BSN moet 9 cijfers bevatten" });
                }

                // Vind de geselecteerde rol (AC6.10.5)
                var role = await _context.Roles.FindAsync(request.RoleId);
                if (role == null)
                    return BadRequest(new { message = "Ongeldige rol geselecteerd" });

                // Maak nieuwe gebruiker aan
                var newUser = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    StreetName = request.StreetName ?? "Niet opgegeven",
                    HouseNumber = request.HouseNumber ?? "Niet opgegeven",
                    PostalCode = request.PostalCode ?? "Niet opgegeven",
                    CitizenServiceNumber = request.CitizenServiceNumber ?? "000000000",
                    DateOfBirth = request.DateOfBirth ?? DateTime.Now.AddYears(-30),
                    Gender = request.Gender ?? "Zeg ik liever niet",
                    PhoneNumber = request.PhoneNumber,
                    RoleId = request.RoleId,
                    CreatedAt = DateTime.UtcNow,
                    // Standaard wachtwoord genereren (moet later gewijzigd worden)
                    PasswordHash = ""
                };

                // Genereer een tijdelijk wachtwoord
                var tempPassword = GenerateTemporaryPassword();
                newUser.PasswordHash = _passwordHasher.HashPassword(newUser, tempPassword);

                // Voor artsen/specialisten extra velden
                if (role.RoleName == "Huisarts" || role.RoleName == "Specialist")
                {
                    newUser.PracticeName = request.PracticeName;
                }

                // AC6.10.4: Opslaan in database
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Nieuwe gebruiker aangemaakt door admin: {newUser.Email}, Rol: {role.RoleName}");

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
                    temporaryPassword = tempPassword // Alleen voor ontwikkeldoeleinden, in productie via email versturen
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fout bij aanmaken gebruiker");
                return StatusCode(500, new { message = "Er is een fout opgetreden bij het aanmaken van de gebruiker", error = ex.Message });
            }
        }

        private string GenerateTemporaryPassword()
        {
            // Genereer een willekeurig wachtwoord van 8 tekens
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}