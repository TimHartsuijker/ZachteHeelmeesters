namespace backend.Models;

public class Gebruiker
{
    public int GebruikersID { get; set; }
    public string Voornaam { get; set; } = string.Empty;
    public string Achternaam { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Wachtwoord { get; set; } = string.Empty;
    public string Straatnaam { get; set; } = string.Empty;
    public string Huisnummer { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string Telefoonnummer { get; set; } = string.Empty;
    public int Rol { get; set; }
    public string? Rolnaam { get; set; }
    public bool Systeembeheerder { get; set; }
}
