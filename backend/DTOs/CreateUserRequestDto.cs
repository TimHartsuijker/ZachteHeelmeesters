namespace backend.DTOs
{
    public class CreateUserRequestDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Password { get; set; }
        public string? StreetName { get; set; }
        public string? HouseNumber { get; set; }
        public string? PostalCode { get; set; }
        public string? CitizenServiceNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public string? PracticeName { get; set; } // Voor artsen/specialisten
    }
}