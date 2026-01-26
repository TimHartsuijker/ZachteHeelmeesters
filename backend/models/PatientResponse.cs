namespace backend.Models
{
    public class PatientResponse
    {
        public int PatientID { get; set; }
        public string Voornaam { get; set; } = string.Empty;
        public string Achternaam { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefoonnummer { get; set; } = string.Empty;
        public string Straatnaam { get; set; } = string.Empty;
        public string Huisnummer { get; set; } = string.Empty;
        public string Postcode { get; set; } = string.Empty;
        public string? Plaats { get; set; }
        public string? BSN { get; set; }
        public string? Geboortedatum { get; set; }
        public string? Geslacht { get; set; }
        public string? Huisartspraktijk { get; set; }
        public string? Huisartsnaam { get; set; }
    }
}
