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

            // 🔑 Sessie-gebaseerde blokkade
            string sessionKey = HttpContext.Session.Id;
            var attempts = loginAttempts.GetOrAdd(sessionKey, new LoginAttempt());

            // 1. Permanente blokkade
            if (attempts.BlockCount >= 3)
            {
                return Unauthorized(new
                {
                    message = "Uw account is permanent geblokkeerd."
                });
            }

            // 2. Tijdelijke blokkade
            if (attempts.IsBlocked())
            {
                var minutesLeft = Math.Ceiling(attempts.BlockTimeLeft());
                return Unauthorized(new
                {
                    message = $"U bent nog {minutesLeft} minuten geblokkeerd."
                });
            }

            // 3. Juiste login
            if (request.Email == correctEmail && request.Wachtwoord == correctPassword)
            {
                attempts.ResetFailsOnly();
                return Ok(new
                {
                    message = "Login ok",
                    redirect = "/dashboard"
                });
            }

            // 4. Foute login
            attempts.RegisterFail();

            if (attempts.FailCount >= 3)
            {
                attempts.Block();
                return Unauthorized(new
                {
                    message = "Uw account is 15 minuten geblokkeerd"
                });
            }

            return Unauthorized(new { message = "Inloggegevens zijn incorrect" });
        }
    }
}
