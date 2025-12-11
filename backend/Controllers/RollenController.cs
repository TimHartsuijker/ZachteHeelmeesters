using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RollenController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public RollenController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Rol>>> GetRollen()
    {
        var rollen = new List<Rol>();
        var connectionString = _configuration.GetConnectionString("DefaultConnection");

        try
        {
            using var connection = new SqlConnection(connectionString);
            await connection.OpenAsync();

            var query = "SELECT rolID, rolnaam FROM rollen ORDER BY rolID";
            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                rollen.Add(new Rol
                {
                    RolID = reader.GetInt32(0),
                    Rolnaam = reader.GetString(1)
                });
            }

            return Ok(rollen);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching roles", error = ex.Message });
        }
    }
}
