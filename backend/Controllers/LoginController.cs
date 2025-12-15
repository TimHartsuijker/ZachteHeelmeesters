using backend.Models;
using Microsoft.AspNetCore.Mvc;


namespace BackendLogin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly string correctEmail = "gebruiker@example.com";
        private readonly string correctPassword = "Wachtwoord123";

        [HttpGet]
        public IActionResult TestEndpoint()
        {
            return Ok(new { message = "API werkt!" });
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            Console.WriteLine(request);

            if (request == null)
            {
                return BadRequest(new { message = "Inloggegevens zijn incorrect" });
            }

            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Wachtwoord))
            {
                return BadRequest(new { message = "Gegevens moeten ingevuld zijn" });
            }

            if (request.Email == correctEmail && request.Wachtwoord == correctPassword)
            {
                return Ok(new
                {
                    message = "Login ok",
                    redirect = "/dashboard" // tijdelijk, kan naar /dashboard later
                });
            }

            return Unauthorized(new { message = "Inloggegevens zijn incorrect" });
        }
    }
}



 

        
