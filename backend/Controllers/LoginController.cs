using BackendLogin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace BackendLogin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly string correctEmail = "gebruiker@example.com";
        private readonly string correctPassword = "Wachtwoord123";

        private static ConcurrentDictionary<string, LoginAttempt> loginAttempts =
            new ConcurrentDictionary<string, LoginAttempt>();

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Wachtwoord))
            {
                return BadRequest(new { message = "gegevens moeten ingevuld zijn" });
            }

            var attempts = loginAttempts.GetOrAdd(request.Email, new LoginAttempt());

            // 1. Permanente blokkade
            if (attempts.BlockCount >= 3)
            {
                return Unauthorized(new
                {
                    message = "Uw account is geblokkeerd. Controleer uw e-mail om uw account te deblokkeren."
                });
            }

            // 2. Tijdelijke blokkade (met resterende tijd)
            if (attempts.IsBlocked())
            {
                var minutesLeft = Math.Ceiling(attempts.BlockTimeLeft());

                return Unauthorized(new
                {
                    message = $"Uw account is nog {minutesLeft} minuten geblokkeerd."
                });
            }

            // 3. Juiste login
            if (request.Email == correctEmail && request.Wachtwoord == correctPassword)
            {
                attempts.Reset();
                return Ok(new
                {
                    message = "login ok",
                    redirect = "/dashboard" // later aanpassen als nodig
                });
            }

            // 4. Foute login
            attempts.RegisterFail();

            if (attempts.FailCount >= 3)
            {
                attempts.Block(); // start 15 min blokkade

                return Unauthorized(new
                {
                    message = "Uw account is 15 minuten geblokkeerd."
                });
            }

            return Unauthorized(new { message = "inloggegevens zijn incorrect" });
        }
    }
}
