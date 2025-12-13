using backend.Models;
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
                return BadRequest(new { message = "Gegevens moeten ingevuld zijn" });
            }

            // Gebruik account als sleutel
            string key = correctEmail; // als je meerdere accounts hebt, gebruik database email
            var attempts = loginAttempts.GetOrAdd(key, new LoginAttempt());

            // Permanente blokkade
            if (attempts.BlockCount >= 3)
            {
                return Unauthorized(new
                {
                    message = "Uw account is permanent geblokkeerd. Controleer uw e-mail om uw account te deblokkeren."
                });
            }

            // Tijdelijke blokkade
            if (attempts.IsBlocked())
            {
                var minutesLeft = Math.Ceiling(attempts.BlockTimeLeft());
                return Unauthorized(new
                {
                    message = $"Uw account is nog {minutesLeft} minuten geblokkeerd."
                });
            }

            // Juiste login
            if (request.Email == correctEmail && request.Wachtwoord == correctPassword)
            {
                attempts.ResetFailsOnly();
                return Ok(new
                {
                    message = "Login ok",
                    redirect = "/dashboard"
                });
            }

            // Foute login
            attempts.RegisterFail();

            if (attempts.FailCount >= 3)
            {
                attempts.Block();
                return Unauthorized(new
                {
                    message = "Uw account is 15 minuten geblokkeerd."
                });
            }

            return Unauthorized(new { message = "Inloggegevens zijn incorrect" });
        }
    }
}
