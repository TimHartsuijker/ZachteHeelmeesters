using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using backend.Models;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public PatientController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET api/patient/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientResponse>> GetPatient(int id)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                var query = @"
                    SELECT 
                        p.gebruikersID as PatientID,
                        p.voornaam as Voornaam,
                        p.achternaam as Achternaam,
                        p.email as Email,
                        p.Straatnaam,
                        p.Huisnummer,
                        p.Postcode,
                        p.Telefoonnummer,
                        NULL as BSN,
                        NULL as Geboortedatum,
                        NULL as Geslacht,
                        NULL as Plaats,
                        hp.huisartsID as HuisartsID,
                        h.voornaam as HuisartsVoornaam,
                        h.achternaam as HuisartsAchternaam
                    FROM gebruikers p
                    LEFT JOIN huisarts_patient hp ON hp.patientID = p.gebruikersID
                    LEFT JOIN gebruikers h ON h.gebruikersID = hp.huisartsID
                    WHERE p.gebruikersID = @PatientID
                ";

                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PatientID", id);

                await using var reader = await command.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return NotFound(new { error = "Patient not found" });
                }

                var patient = new PatientResponse
                {
                    PatientID = reader.GetInt32(reader.GetOrdinal("PatientID")),
                    Voornaam = reader.GetString(reader.GetOrdinal("Voornaam")),
                    Achternaam = reader.GetString(reader.GetOrdinal("Achternaam")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Straatnaam = reader.GetString(reader.GetOrdinal("Straatnaam")),
                    Huisnummer = reader.GetString(reader.GetOrdinal("Huisnummer")),
                    Postcode = reader.GetString(reader.GetOrdinal("Postcode")),
                    Telefoonnummer = reader.GetString(reader.GetOrdinal("Telefoonnummer")),
                    BSN = reader.IsDBNull(reader.GetOrdinal("BSN")) ? null : reader.GetString(reader.GetOrdinal("BSN")),
                    Geboortedatum = reader.IsDBNull(reader.GetOrdinal("Geboortedatum")) ? null : reader.GetString(reader.GetOrdinal("Geboortedatum")),
                    Geslacht = reader.IsDBNull(reader.GetOrdinal("Geslacht")) ? null : reader.GetString(reader.GetOrdinal("Geslacht")),
                    Plaats = reader.IsDBNull(reader.GetOrdinal("Plaats")) ? null : reader.GetString(reader.GetOrdinal("Plaats")),
                    Huisartspraktijk = null, // niet in DB
                    Huisartsnaam = !reader.IsDBNull(reader.GetOrdinal("HuisartsVoornaam")) && !reader.IsDBNull(reader.GetOrdinal("HuisartsAchternaam"))
                        ? $"{reader.GetString(reader.GetOrdinal("HuisartsVoornaam"))} {reader.GetString(reader.GetOrdinal("HuisartsAchternaam"))}"
                        : null
                };

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Server error", details = ex.Message });
            }
        }
    }
}
