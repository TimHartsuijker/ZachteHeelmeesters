namespace backend.Models
{
    public class RegisterRequest
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string StreetName { get; set; } = null!;
        public string HouseNumber { get; set; } = null!;
        public string PostalCode { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string CitizenServiceNumber { get; set; } = null!;
        public int DoctorId { get; set; } = 0;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = null!;
    }
}
