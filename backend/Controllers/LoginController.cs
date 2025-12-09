using BackendLogin.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendLogin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly string correctEmail = "gebruiker@example.com";
        private readonly string correctPassword = "Wachtwoord123";

        // GET api/login
        [HttpGet]
        public IActionResult TestEndpoint()
        {
            return Ok(new { message = "API werkt!" });
        }

        // POST api/login
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            Console.WriteLine(request);

            if (request == null)
            {
                return BadRequest(new { message = "inloggegevens zijn incorrect" });
            }

            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Wachtwoord))
            {
                return BadRequest(new { message = "gegevens moeten ingevuld zijn" });
            }

            if (request.Email == correctEmail && request.Wachtwoord == correctPassword)
            {
                return Ok(new
                {
                    message = "login ok",
                    token = "FAKE-JWT-TOKEN",
                    redirect = "/" // tijdelijk, kan naar /dashboard later
                });
            }

            return Unauthorized(new { message = "inloggegevens zijn incorrect" });
        }
    }
}
