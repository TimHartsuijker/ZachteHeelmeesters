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
                request.DateOfBirth == default ||
                string.IsNullOrWhiteSpace(request.Gender))
            {
                return BadRequest(new { message = "All fields must be filled in" });
            }

            // 2️⃣ Controleer of email al bestaat
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest(new { message = "The email that was used is already registered" });
            }

            // 3️⃣ Controleer of CitizenServiceNumber al bestaat
            if (_context.Users.Any(u => u.CitizenServiceNumber == request.CitizenServiceNumber))
            {
                return BadRequest(new { message = "The Citizen Service Number is already registered" });
            }

            // 4️⃣ Haal Patient role op
            var patientRole = _context.Roles.FirstOrDefault(r => r.RoleName == "Patient");
            if (patientRole == null)
            {
                return BadRequest(new { message = "Patient role not found. Please seed roles first." });
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
                Gender = request.Gender,
                CreatedAt = DateTime.UtcNow,
                RoleId = patientRole.Id
            };

            // 6️⃣ Hash wachtwoord
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            // 7️⃣ Opslaan in database
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "User registered successfully" });
        }
    }
}