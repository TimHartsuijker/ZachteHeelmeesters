using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;

        public RegisterController(AppDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        [HttpPost]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // 1️⃣ Controleer of alle verplichte velden zijn ingevuld
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.StreetName) ||
                string.IsNullOrWhiteSpace(request.HouseNumber) ||
                string.IsNullOrWhiteSpace(request.PostalCode) ||
                string.IsNullOrWhiteSpace(request.CitizenServiceNumber) ||
                request.DoctorId == 0 ||
                request.DateOfBirth == default ||
                string.IsNullOrWhiteSpace(request.Gender))
            {
                return BadRequest(new { message = "Alle velden moeten ingevuld zijn." });
            }

            // 2️⃣ Controleer of email al bestaat
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "Dit email is al geregistreerd." });
            }

            // 3️⃣ Controleer of CitizenServiceNumber al bestaat
            if (_context.Users.Any(u => u.CitizenServiceNumber == request.CitizenServiceNumber))
            {
                return BadRequest(new { message = "Dit BSN nummer is al geregistreerd." });
            }

            // Controleer of huisarts bestaat
            var doctor = _context.Users.FirstOrDefault(u => u.Id == request.DoctorId);
            if (doctor == null)
            {
                return BadRequest(new { message = "De opgegeven huisarts bestaat niet." });
            }

            // 4️⃣ Haal Patient role op
            var patientRole = _context.Roles.FirstOrDefault(r => r.RoleName == "Patiënt");
            if (patientRole == null)
            {
                return BadRequest(new { message = "Rol Patiënt niet gevonden, seed de database." });
            }

            // 5️⃣ Maak nieuwe user aan
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                StreetName = request.StreetName,
                HouseNumber = request.HouseNumber,
                PostalCode = request.PostalCode,
                PhoneNumber = request.PhoneNumber,
                CitizenServiceNumber = request.CitizenServiceNumber,
                DateOfBirth = request.DateOfBirth,
                DoctorId = doctor.Id,
                Gender = request.Gender,
                CreatedAt = DateTime.UtcNow,
                RoleId = patientRole.Id
            };

            // 6️⃣ Hash wachtwoord
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            // 7️⃣ Opslaan in database
            _context.Users.Add(user);
            _context.SaveChanges();

            return Created("/" , new { message = "Gebruiker correct geregistreerd!" });
        }
    }
}