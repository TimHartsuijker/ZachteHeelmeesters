namespace backend.Models
{
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Wachtwoord { get; set; }
    }
}
