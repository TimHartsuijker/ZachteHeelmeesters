using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GebruikersController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public GebruikersController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Gebruiker>>> GetGebruikers()
    {
        var gebruikers = new List<Gebruiker>();
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT 
                    g.gebruikersID, 
                    g.voornaam, 
                    g.achternaam, 
                    g.email, 
                    g.Straatnaam,
                    g.Huisnummer,
                    g.Postcode,
                    g.Telefoonnummer,
                    g.rol,
                    r.rolnaam,
                    g.systeembeheerder
                FROM gebruikers g
                LEFT JOIN rollen r ON g.rol = r.rolID
                ORDER BY g.gebruikersID";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                gebruikers.Add(new Gebruiker
                {
                    GebruikersID = reader.GetInt32(0),
                    Voornaam = reader.GetString(1),
                    Achternaam = reader.GetString(2),
                    Email = reader.GetString(3),
                    Straatnaam = reader.GetString(4),
                    Huisnummer = reader.GetString(5),
                    Postcode = reader.GetString(6),
                    Telefoonnummer = reader.GetString(7),
                    Rol = reader.GetInt32(8),
                    Rolnaam = reader.IsDBNull(9) ? null : reader.GetString(9),
                    Systeembeheerder = reader.GetBoolean(10)
                });
            }

            return Ok(gebruikers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching users", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateGebruiker(int id, [FromBody] Gebruiker gebruiker)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE gebruikers 
                SET rol = @rol, systeembeheerder = @systeembeheerder
                WHERE gebruikersID = @id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@rol", gebruiker.Rol);
            command.Parameters.AddWithValue("@systeembeheerder", gebruiker.Systeembeheerder);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new { message = "User updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating user", error = ex.Message });
        }
    }
}
